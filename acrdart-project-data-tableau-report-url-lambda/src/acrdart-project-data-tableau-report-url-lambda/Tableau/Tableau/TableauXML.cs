using Acr.Tableau.Constants;
using Acr.Tableau.Interfaces;
using System.Xml;
using TableauLambda.Data;

namespace Acr.Tableau.Tableau
{
    class TableauXML : ITableauXML
    {
        private string _site = Environment.GetEnvironmentVariable(AppSettingKeys.TableauSite);
        private string _group = Environment.GetEnvironmentVariable(AppSettingKeys.TableauGroup);
        private readonly string _adminUserName;
        private readonly string _adminPassword;
        private readonly string _userPassWord;

        public TableauXML(IGenericSecret genericSecret) 
        {
            _adminUserName = genericSecret.TableauAdminUserName;
            _adminPassword = genericSecret.TableauAdminPassword;
            _userPassWord = genericSecret.TableauUserPassword;
        }

        #region Public methods

        /// <summary>
        /// To create xml for the user sign to tableau
        /// </summary>
        /// <param name="isAdmin">IsAdmin</param>
        /// <param name="userName">Username</param>
        /// <returns></returns>
        public string TableauSignIn(bool isAdmin, string userName)
        {
            var xmlDoc = new XmlDocument();
            string xml = string.Empty;
            var tsRequest = xmlDoc.CreateElement(TableauConstants.TsRequest);
            var user = xmlDoc.CreateElement(TableauConstants.Credentials);
            user.SetAttribute(TableauConstants.Name, isAdmin ? _adminUserName : userName);
            user.SetAttribute(TableauConstants.TableauPassword, isAdmin ? _adminPassword : _userPassWord);
            var site = xmlDoc.CreateElement(TableauConstants.Site);
            site.SetAttribute(TableauConstants.ContentUrl, _site);
            user.AppendChild(site);
            tsRequest.AppendChild(user);
            xmlDoc.AppendChild(tsRequest);
            xml = xmlDoc.InnerXml;
            xmlDoc = null;
            tsRequest = null;
            user = null;
            site = null;
            return xml;
        }

        /// <summary>
        /// To parse the xml response to get token and siteid
        /// </summary>
        /// <param name="token">Authenticated token</param>
        /// <param name="siteID">SiteId</param>
        /// <param name="xml">xml</param>
        public void ParseXML(out string token, out string siteID, string xml)
        {
            token = string.Empty;
            siteID = string.Empty;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeList = xmlDoc.GetElementsByTagName(TableauConstants.Credentials);
            foreach (XmlNode node in nodeList)
            {
                token = node.Attributes[TableauConstants.TableauToken].InnerText;
            }
            nodeList = xmlDoc.GetElementsByTagName(TableauConstants.Site);
            foreach (XmlNode node in nodeList)
            {
                siteID = node.Attributes[TableauConstants.ID].InnerText;
            }
            xmlDoc = null;
            nodeList = null;
            xml = string.Empty;
        }

        /// <summary>
        /// To parse the xml response and get the users
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns></returns>
        public List<TableauUser> GetUsers(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeList = xmlDoc.GetElementsByTagName(TableauConstants.User);
            List<TableauUser> users = new List<TableauUser>();
            foreach (XmlNode node in nodeList)
            {
                TableauUser user = new TableauUser();
                user.UserName = node.Attributes[TableauConstants.Name].InnerText;
                user.UserID = node.Attributes[TableauConstants.ID].InnerText;
                if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.UserID))
                    users.Add(user);
            }
            xmlDoc = null;
            nodeList = null;
            xml = string.Empty;
            return users;
        }

        /// <summary>
        /// Create the xml format to add and update the user 
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="isUpdate">Isupdate</param>
        /// <param name="emailID">EmailId</param>
        /// <returns></returns>
        public string AddUserToSite(string userName, bool isUpdate, string emailID)
        {
            var xmlDoc = new XmlDocument();
            string xml = string.Empty;
            var tsRequest = xmlDoc.CreateElement(TableauConstants.TsRequest);
            var user = xmlDoc.CreateElement(TableauConstants.User);
            if (isUpdate)
            {
                user.SetAttribute(TableauConstants.FullName, userName);
                user.SetAttribute(TableauConstants.Email, emailID);
                user.SetAttribute(TableauConstants.TableauPassword, _userPassWord);
                user.SetAttribute(TableauConstants.SiteRole, TableauConstants.TableauUserRole);
            }
            else
            {
                user.SetAttribute(TableauConstants.Name, userName);
                user.SetAttribute(TableauConstants.SiteRole, TableauConstants.TableauUserRole);
                user.SetAttribute(TableauConstants.ContentAdmin, TableauConstants.HasRights);
                user.SetAttribute(TableauConstants.SupressGettingStarted, TableauConstants.HasRights);
            }
            tsRequest.AppendChild(user);
            xmlDoc.AppendChild(tsRequest);
            xml = xmlDoc.InnerXml;
            xmlDoc = null;
            tsRequest = null;
            user = null;
            return xml;
        }

        /// <summary>
        /// To get the UserId from the response
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="userID">UserId</param>
        public void GetTableauUserID(string xml, out string userID)
        {
            userID = string.Empty;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeList = xmlDoc.GetElementsByTagName(TableauConstants.User);
            foreach (XmlNode node in nodeList)
            {
                userID = node.Attributes[TableauConstants.ID].InnerText;
            }
            xmlDoc = null;
            nodeList = null;
            xml = string.Empty;
        }

        /// <summary>
        /// To get the groupId from the response
        /// </summary>
        /// <param name="xml">xml</param>
        /// <returns></returns>
        public string GetGroupID(string xml)
        {
            string groupID = string.Empty;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var nodeList = xmlDoc.GetElementsByTagName(TableauConstants.Group);
            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes[TableauConstants.Name].InnerText == _group)
                {
                    groupID = node.Attributes[TableauConstants.ID].InnerText;
                    break;
                }
            }
            xmlDoc = null;
            nodeList = null;
            xml = string.Empty;
            return groupID;
        }

        /// <summary>
        /// Create the xml format to add the user to the group
        /// </summary>
        /// <param name="userID">UserId</param>
        /// <returns></returns>
        public string AddUserToGroup(string userID)
        {
            var xmlDoc = new XmlDocument();
            string xml = string.Empty;
            var tsRequest = xmlDoc.CreateElement(TableauConstants.TsRequest);
            var user = xmlDoc.CreateElement(TableauConstants.User);
            user.SetAttribute(TableauConstants.ID, userID);
            tsRequest.AppendChild(user);
            xmlDoc.AppendChild(tsRequest);
            xml = xmlDoc.InnerXml;
            xmlDoc = null;
            tsRequest = null;
            user = null;
            return xml;
        }



        #endregion
    }
}
