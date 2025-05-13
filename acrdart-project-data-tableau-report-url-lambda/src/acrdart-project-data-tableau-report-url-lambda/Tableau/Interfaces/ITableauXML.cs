using Acr.Tableau.Tableau;

namespace Acr.Tableau.Interfaces
{
    internal interface ITableauXML
    {
        string TableauSignIn(bool isAdmin, string userName);

        void ParseXML(out string token, out string siteID, string xml);

        List<TableauUser> GetUsers(string xml);

        string AddUserToSite(string userName, bool isUpdate, string emailID);

        void GetTableauUserID(string xml, out string userID);

        string GetGroupID(string xml);

        string AddUserToGroup(string userID);
    }
}
