namespace TableauLambda.Infrastructures
{
    public interface ISecretExtensionProvider
    {
        Task<T?> GetSecretAsync<T>(string secretName, CancellationToken cancellationToken = default) where T : class;
    }
}
