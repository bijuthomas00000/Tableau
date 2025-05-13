using Amazon.SecretsManager.Model;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using System.Net.Http.Json;

namespace TableauLambda.Infrastructures
{
    public class SecretManagerExtensionProvider : ISecretExtensionProvider
    {
        private readonly HttpClient _httpClient;

        private readonly string GetSecretsEndpoint = "/secretsmanager/get?secretId=";

        public SecretManagerExtensionProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetSecretAsync<T>(string secretName, CancellationToken cancellationToken = default) where T : class
        {
            var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri($"{GetSecretsEndpoint}{HttpUtility.UrlEncode(secretName)}", UriKind.Relative));

            httpRequest.Headers.Add("X-Aws-Parameters-Secrets-Token",
                Environment.GetEnvironmentVariable("AWS_SESSION_TOKEN")
            );
            var response = await _httpClient
                .SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseAsJson = await response.Content.ReadFromJsonAsync<GetSecretValueResponse>(cancellationToken);
            if (typeof(T) == typeof(string))
            {
                return responseAsJson!.SecretString as T;
            }

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            T secretValue = JsonSerializer.Deserialize<T>(responseAsJson!.SecretString, jsonSerializerOptions)!;
            return secretValue;
        }
    }
}
