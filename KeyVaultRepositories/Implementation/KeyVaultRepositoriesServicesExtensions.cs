using System;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.KeyVault.Client;
using Microsoft.KeyVault.WebKey;

namespace KeyVaultRepositories.Implementation
{
    public static class KeyVaultRepositoriesServicesExtensions
    {
        public static void AddKeyVaultRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            

        }
        public static string GetAccessToken(IConfiguration configuration, string authority, string resource, string scope)
        {
            var client_id = configuration["AuthClientId"];
            var client_secret = configuration["AuthClientSecret"];

            var clientCredential = new ClientCredential(client_id, client_secret);
            var context = new AuthenticationContext(authority, null);
            var result = context.AcquireToken(resource, clientCredential);

            return result.AccessToken;
        }
    }
}