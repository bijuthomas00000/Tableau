namespace TableauLambda.Data
{
    public interface IGenericSecret
    {
        string BoomiApiKey { get; set; }
        string EncryptSaltValue { get; set; }
        string EncryptPassPhrase { get; set; }
        string EncryptInitVector { get; set; }
        string TableauAdminUserName { get; set; }
        string TableauAdminPassword { get; set; }
        string TableauUserPassword { get; set; }
    }
}
