using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using Acr.Tableau.Tableau;
using Acr.Tableau.Interfaces;
using TableauLambda.Validators.Exceptions;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AcrTableauLambda
{
    public class TableauDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> WorkBooks { get; set; }
        public List<string> SheetNames { get; set; }
    }

    public class Function
    {
        private readonly ITableauWebRequest _tableauWebRequest;

        public Function(ITableauWebRequest tableauWebRequest)
        {
            _tableauWebRequest = tableauWebRequest;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var tableauDto = JsonSerializer.Deserialize<TableauDto>(request.Body);

                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        if (tableauDto == null || tableauDto.WorkBooks == null || tableauDto.SheetNames == null ||
                            tableauDto.WorkBooks.Count == 0 || tableauDto.SheetNames.Count == 0 ||
                            tableauDto.WorkBooks.Count != tableauDto.SheetNames.Count)
                        {
                            return new APIGatewayProxyResponse
                            {
                                StatusCode = 400,
                                Body = "Please provide Tableau values",
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }

                        await _tableauWebRequest.AddUserToTableau(tableauDto.UserName, tableauDto.Email);
                        var responses = new List<object>();
                        for (int j = 0; j < tableauDto.WorkBooks.Count; j++)
                        {
                            var response = await _tableauWebRequest.GetTrustedToken(
                                tableauDto.UserName,
                                tableauDto.WorkBooks[j],
                                tableauDto.SheetNames[j]);
                            responses.Add(response);
                        }
                        return new APIGatewayProxyResponse
                        {
                            StatusCode = 200,
                            Body = JsonSerializer.Serialize(responses),
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                        };
                    }
                    catch (ValidationFailedException ex)
                    {
                        return new APIGatewayProxyResponse
                        {
                            StatusCode = 400,
                            Body = JsonSerializer.Serialize(new { errors = ex.Errors }),
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                        };
                    }
                    catch (Exception ex)
                    {
                        if (i == 4)
                        {
                            return new APIGatewayProxyResponse
                            {
                                StatusCode = 500,
                                Body = "An unhandled error occurred",
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }
                    }
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = "Maximum retry attempts reached",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = "An unexpected error occurred",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
    }
}
