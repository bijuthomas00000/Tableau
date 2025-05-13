namespace TableauLambda.Data
{
    public class GenericSecret : IGenericSecret
    {
        public string BoomiApiKey { get; set; }
        public string EncryptSaltValue { get; set; }
        public string EncryptPassPhrase { get; set; }
        public string EncryptInitVector { get; set; }
        public string TableauAdminUserName { get; set; }
        public string TableauAdminPassword { get; set; }
        public string TableauUserPassword { get; set; }

    }
}
