using Acr.Tableau.Interfaces;
using Acr.Tableau.Tableau;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TableauLambda.Data;
using TableauLambda.Infrastructures;


namespace AcrTableauLambda;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {


        services.AddScoped<ISecretExtensionProvider, SecretManagerExtensionProvider>();
        services.AddScoped<ISecretExtensionProvider, SecretManagerExtensionProvider>();

        services.AddTransient<ITableauWebRequest, TableauWebRequest>();

        services.AddSingleton<IGenericSecret>(provider =>
        {
            var secretProvider = provider.GetService<ISecretExtensionProvider>();
            return new GenericSecret()
            {
                //BoomiApiKey = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("BoomiApiSecretName"), default).Result,
                //EncryptInitVector = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("EncryptInitVectorSecretName"), default).Result,
                //EncryptPassPhrase = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("EncryptPassPhraseSecretName"), default).Result,
                //EncryptSaltValue = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("EncryptSaltValueSecretName"), default).Result,
                TableauAdminUserName = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("TableauAdminUserSecretName"), default).Result,
                TableauAdminPassword = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("TableauAdminPasswordSecretName"), default).Result,
                TableauUserPassword = secretProvider.GetSecretAsync<string>(Environment.GetEnvironmentVariable("TableauUserPasswordSecretName"), default).Result

            };
        });

    }
}
