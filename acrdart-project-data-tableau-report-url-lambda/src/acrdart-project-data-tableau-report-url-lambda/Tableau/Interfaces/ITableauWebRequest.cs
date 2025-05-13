namespace Acr.Tableau.Interfaces
{
    public interface ITableauWebRequest
    {
        Task AddUserToTableau(string userName, string email);

        Task<string> GetTrustedToken(string userName, string workBook, string sheetName);

        Task TableauSignOut(string token, string userName);
    }
}
