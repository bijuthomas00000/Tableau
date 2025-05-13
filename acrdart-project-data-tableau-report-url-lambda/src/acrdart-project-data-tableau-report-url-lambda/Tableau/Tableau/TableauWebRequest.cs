using Acr.Tableau.Constants;
using Acr.Tableau.Interfaces;
using System.Net;
using System.Text;
using TableauLambda.Data;

namespace Acr.Tableau.Tableau
{
    public class TableauWebRequest : ITableauWebRequest
    {
        private static string _tableauHostName = Environment.GetEnvironmentVariable(AppSettingKeys.TableauHostName);
        private string _site = Environment.GetEnvironmentVariable(AppSettingKeys.TableauSite);
        private ITableauXML _tableauXML;
        private static readonly HttpClient httpClient = new HttpClient();
        private Security.Tableau.TableauWebRequest _webRequest;

        public TableauWebRequest(IGenericSecret genericSecret)
        {
            _tableauXML = new TableauXML(genericSecret);
            _webRequest = new Security.Tableau.TableauWebRequest();
        }

        #region Private methods
        /// <summary>
        /// To sign in the user to tableau server
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        private string SignInUser(string xml, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.Auth + TableauConstants.Character + TableauConstants.Signin;
            return _webRequest.GetTableauResponse(url, TableauConstants.Post, string.Empty, xml, userName, true, string.Empty, string.Empty);
        }

        /// <summary>
        /// To get the users from a site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        private async Task<string> GetUsersTableau(string token, string siteID, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauUsers + TableauConstants.Character;
            return await GetTableauResponseAsync(url, TableauConstants.Get, token, string.Empty, TableauConstants.RetrieveUsers, userName, true, siteID, string.Empty);
        }

        /// <summary>
        /// To add the user to a site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="xml">xml</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        private async Task<string> AddUserToSite(string token, string siteID, string xml, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauUsers;
            return await GetTableauResponseAsync(url, TableauConstants.Post, token, xml, TableauConstants.AddUserToSite, userName, true, siteID, string.Empty);
        }

        /// <summary>
        /// Update the user details in the Tableau server
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="xml">xml</param>
        /// <param name="userName">Username</param>
        private void UpdateUser(string token, string siteID, string userID, string xml, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauUsers + TableauConstants.Character + userID;
            _webRequest.GetTableauResponse(url, TableauConstants.Put, token, xml, userName, false, siteID, userID);
        }

        /// <summary>
        /// To get groupId
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        private async Task<string> GetGroupID(string token, string siteID, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauGroups;
            return await GetTableauResponseAsync(url, TableauConstants.Get, token, string.Empty, TableauConstants.GetGroupID, userName, true, siteID, string.Empty);
        }

        /// <summary>
        /// To get the users from the group/to add a user to the group
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="groupID">GroupId</param>
        /// <param name="Type">Request Type</param>
        /// <param name="xml">xml</param>
        /// <param name="userName">Username</param>
        /// <param name="userID">UserId</param>
        /// <returns></returns>
        private async Task<string> GetOrAddUsersToGroup(string token, string siteID, string groupID, string Type, string xml, string userName, string userID)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauGroups + TableauConstants.Character + groupID + TableauConstants.Character + TableauConstants.TableauUsers;
            string purpose = string.Empty;
            if (Type == TableauConstants.Get)
                purpose = TableauConstants.RetrieveUsersFromGroup;
            else
                purpose = TableauConstants.AddUserToGroup;

            return await GetTableauResponseAsync(url, Type, token, xml, purpose, userName, true, siteID, userID);
        }


        /// <summary>
        /// To get the workbooks from the site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>

        private async Task<string> GetUserWorkBooks(string token, string siteID, string userID, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauUsers + TableauConstants.Character + userID + TableauConstants.Character + TableauConstants.TableauWorkBooks;
            return await GetTableauResponseAsync(url, TableauConstants.Get, token, string.Empty, TableauConstants.GetUserWorkBooks, userName, true, siteID, userID);
        }

        /// <summary>
        /// To remove the user from the site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="userName">Username</param>
        private static void RemoveUserFromSite(string token, string siteID, string userID, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Version + TableauConstants.Character + TableauConstants.TableauSites + TableauConstants.Character + siteID + TableauConstants.Character + TableauConstants.TableauUsers + TableauConstants.Character + userID;
            GetTableauResponseAsync(url, TableauConstants.Delete, token, string.Empty, TableauConstants.RemoveUserFromSite, userName, false, siteID, userID).GetAwaiter().GetResult();
        }

        /// <summary>
        /// To add the user in tableau
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userName">Username</param>
        /// <param name="userID">UserId</param>
        /// <param name="TableauUserName">TableauUsername</param>
        /// <param name="emailID">EamilId</param>
        private void AddUserTableau(string token, string siteID, string userName, out string userID, out string TableauUserName, string emailID)
        {
            userID = string.Empty;
            TableauUserName = string.Empty;
            userName = !string.IsNullOrEmpty(userName) ? userName : userName.ToLower();
            List<TableauUser> users = new List<TableauUser>();
            var response = GetUsersTableau(token, siteID, userName).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(response))
            {
                users = _tableauXML.GetUsers(response);
            }
            if (!users.Where(i => i.UserName.ToLower() == userName).Any())
            {
                AddUserToSiteTableau(token, userName, siteID, out userID, emailID);
                TableauUserName = userName;
                UpdateUserTableau(token, TableauUserName, siteID, userID, emailID);
                AddUserToGroupTableau(token, siteID, userID, TableauUserName);
            }
            else
            {
                userID = users.Where(i => i.UserName.ToLower() == userName).Select(i => i.UserID).FirstOrDefault();
                TableauUserName = users.Where(i => i.UserName.ToLower() == userName).Select(i => i.UserName).FirstOrDefault();
            }
        }

        /// <summary>
        /// To login the Admin into tableau site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userName">Username</param>
        private void AdminSignIn(out string token, out string siteID, string userName)
        {
            token = string.Empty;
            siteID = string.Empty;
            var xml = _tableauXML.TableauSignIn(true, userName);
            var response = SignInUser(xml, userName);
            if (!string.IsNullOrEmpty(response))
            {
                _tableauXML.ParseXML(out token, out siteID, response);
            }
        }

        /// <summary>
        /// Add the user to the site
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="userName">Username</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="emailID">EmailId</param>
        private void AddUserToSiteTableau(string token, string userName, string siteID, out string userID, string emailID)
        {
            userID = string.Empty;
            var xml = _tableauXML.AddUserToSite(userName, false, emailID);
            var response = AddUserToSite(token, siteID, xml, userName).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(response))
            {
                _tableauXML.GetTableauUserID(response, out userID);
            }
        }

        /// <summary>
        /// Add the user to the group
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="userName">Username</param>
        private async Task AddUserToGroupTableau(string token, string siteID, string userID, string userName)
        {
            string groupID = string.Empty;
            var response = await GetGroupID(token, siteID, userName);
            if (!string.IsNullOrEmpty(response))
            {
                groupID = _tableauXML.GetGroupID(response);
            }
            if (!string.IsNullOrEmpty(groupID))
                await CheckUserInGroup(token, siteID, groupID, userID, userName);
        }

        /// <summary>
        /// To update the user details
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="userName">Username</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <param name="emailID">EmailId</param>
        private void UpdateUserTableau(string token, string userName, string siteID, string userID, string emailID)
        {
            var xml = _tableauXML.AddUserToSite(userName, true, emailID);
            UpdateUser(token, siteID, userID, xml, userName);
        }

        /// <summary>
        /// To check whether the user exists in the group
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="groupID">GroupId</param>
        /// <param name="userID">UserId</param>
        /// <param name="userName">Username</param>
        private async Task CheckUserInGroup(string token, string siteID, string groupID, string userID, string userName)
        {
            List<TableauUser> users = new List<TableauUser>();
            var response = await GetOrAddUsersToGroup(token, siteID, groupID, TableauConstants.Get, string.Empty, userName, userID);
            if (!string.IsNullOrEmpty(response))
            {
                users = _tableauXML.GetUsers(response);
            }

            if (!users.Where(i => i.UserID == userID).Any())
            {
                await AddUser(token, siteID, groupID, userID, userName);
            }
        }

        /// <summary>
        /// Add the user to the group if not exists
        /// </summary>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="groupID">GroupId</param>
        /// <param name="userID">Username</param>
        /// <param name="userName">Username</param>
        private async Task AddUser(string token, string siteID, string groupID, string userID, string userName)
        {
            var xml = _tableauXML.AddUserToGroup(userID);
            await GetOrAddUsersToGroup(token, siteID, groupID, TableauConstants.Post, xml, userName, userID);
        }

        /// <summary>
        /// To get the response from the tableau
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="requestType">Request Type</param>
        /// <param name="token">Admin authenticated token</param>
        /// <param name="xml">xml</param>
        /// <param name="purpose">Purpose</param>
        /// <param name="userName">Username</param>
        /// <param name="isResponse">Response</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="userID">UserId</param>
        /// <returns></returns>
        private static string GetTableauResponse(string url, string requestType, string token, string xml, string purpose, string userName, bool isResponse, string siteID, string userID)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = null;
            byte[] buffer = null;
            string xmlResponse = string.Empty;
            try
            {
                request.Method = requestType;
                request.ContentType = TableauConstants.ContentType;
                request.CookieContainer = new CookieContainer();
                if (!string.IsNullOrEmpty(token))
                    request.Headers.Add(TableauConstants.TableauHeader, token);
                if (!string.IsNullOrEmpty(xml))
                {
                    buffer = Encoding.UTF8.GetBytes(xml);
                    request.ContentLength = buffer.Length;
                    //request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                request.ServicePoint.ConnectionLeaseTimeout = 0;
                request.ServicePoint.MaxIdleTime = 0;
                request.Timeout = 10000; // Timeout in milliseconds (10 seconds)
                request.ReadWriteTimeout = 10000;
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                        using (var receiveStream = response.GetResponseStream())
                        {
                            using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                            {
                                xmlResponse = readStream.ReadToEnd();
                            }
                        }
                };
            }
            catch (Exception ex)
            {
                if (requestType == TableauConstants.Post && purpose == TableauConstants.AddUserToGroup)
                    RemoveUserFromSite(token, siteID, userID, userName);
                throw ex;
            }
            finally
            {
                request = null;
                buffer = null;
                if (!isResponse)
                    xmlResponse = string.Empty;
            }
            return xmlResponse;
        }

        private static async Task<string> GetTableauResponseAsync(string url, string requestType, string token, string xml, string purpose, string userName, bool isResponse, string siteID, string userID)
        {
            var handler = new HttpClientHandler
            {
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13
            };
            using var httpClient = new HttpClient(handler);
            if (!string.IsNullOrEmpty(token))
                httpClient.DefaultRequestHeaders.Add(TableauConstants.TableauHeader, token);

            HttpContent content = null;
            if (!string.IsNullOrEmpty(xml))
                content = new StringContent(xml, Encoding.UTF8, TableauConstants.ContentType);

            HttpResponseMessage response = null;
            if (requestType.Equals("POST", StringComparison.OrdinalIgnoreCase))
                response = await httpClient.PostAsync(url, content);
            else if (requestType.Equals("GET", StringComparison.OrdinalIgnoreCase))
                response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode(); // Throw if not successful

            return await response.Content.ReadAsStringAsync();
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Add the user to tableau server
        /// </summary>
        /// <param name="UserName">Username</param>
        /// <param name="Email">EmailId</param>
        public async Task AddUserToTableau(string UserName, string Email)
        {
            string token, siteID, tableauUserID, TableauUserName = string.Empty;
            AdminSignIn(out token, out siteID, UserName);
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(siteID))
            {
                AddUserTableau(token, siteID, UserName.ToLower(), out tableauUserID, out TableauUserName, Email);
                await TableauSignOut(token, UserName);
            }
        }

        /// <summary>
        /// To get the trusted token from tableau
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="report">Reportname</param>
        /// <returns></returns>
        public async Task<string> GetTrustedToken(string userName, string workBook, string sheetName)
        {
            string workBookUrl = string.Empty;
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Trusted + TableauConstants.Question + TableauConstants.UserName + TableauConstants.Equals + userName + TableauConstants.And + TableauConstants.TargetSite + TableauConstants.Equals + _site;
            string trustedAuthToken = await GetTableauResponseAsync(url, TableauConstants.Post, string.Empty, string.Empty, TableauConstants.GetTrustedToken, userName, true, string.Empty, string.Empty);
            if (!string.IsNullOrEmpty(trustedAuthToken) && trustedAuthToken != "-1")
                return workBookUrl = _tableauHostName + TableauConstants.Character + TableauConstants.Trusted + TableauConstants.Character + trustedAuthToken + TableauConstants.Character + TableauConstants.T + TableauConstants.Character + _site + TableauConstants.Character + TableauConstants.Views + TableauConstants.Character + workBook + TableauConstants.Character + sheetName + TableauConstants.CustomViews;
            else
                return trustedAuthToken;
        }

        /// <summary>
        /// To signout the user from the tableau
        /// </summary>
        /// <param name="token">Authenticated token</param>
        /// <param name="userName">Username</param>
        public async Task TableauSignOut(string token, string userName)
        {
            string url = _tableauHostName + TableauConstants.Character + TableauConstants.Api + TableauConstants.Character + TableauConstants.Version + TableauConstants.Character + TableauConstants.Auth + TableauConstants.Character + TableauConstants.SignOut;
            await GetTableauResponseAsync(url, TableauConstants.Post, token, string.Empty, TableauConstants.SignOutUser, userName, false, string.Empty, string.Empty);
        }

        #endregion
    }
}
