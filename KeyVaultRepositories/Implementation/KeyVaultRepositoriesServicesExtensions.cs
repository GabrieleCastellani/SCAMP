using System;
using System.IO.Pipes;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.KeyVault.Client;

namespace KeyVaultRepositories.Implementation
{
    public static class KeyVaultRepositoriesServicesExtensions
    {
        public static void  AddKeyVaultRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            KeyVaultScampClient kvsvc = new KeyVaultScampClient(configuration);
            services.AddInstance(kvsvc);
            services.AddTransient<IKeyRepository, KeyRepository>();

        }
    }

   public class KeyVaultScampClient
    {
       private readonly IConfiguration _configuration;
       private readonly KeyVaultClient _keyVaultClient;
       private readonly string _keyVaultUrl;
       private static X509Certificate2 clientAssertionCertPfx;

        public KeyVaultScampClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _keyVaultUrl = _configuration["KeyVault:Url"];

#if DEBUG
            //This should be used only during development
            _keyVaultClient = new KeyVaultClient(GetAccessToken, setRequestUriCallback: SetRequestUri);
#else
            //This use certificate service in Azure platform and doesn't need shared keys
            _keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            clientAssertionCertPfx = FindCertificateByThumbprint(_configuration["KeyVault:AuthCertThumbprintSetting"]);
#endif
        }

        public KeyVaultClient GetClient()
       {
           return _keyVaultClient;
       }
        public string  GetKeyVaultUrl()
        {
            return _keyVaultUrl;
        }
        public  string GetAccessTokenCert(string authority, string resource, string scope)
        {
            var clientId = _configuration["KeyVault:AuthClientId"];

            var context = new AuthenticationContext(authority, null);

            var assertionCert = new ClientAssertionCertificate(clientId, clientAssertionCertPfx);

            var result = context.AcquireToken(resource, assertionCert);

            return result.AccessToken;
        }
        public  string GetAccessToken(string authority, string resource, string scope)
        {
            var clientId = _configuration["KeyVault:AuthClientId"];
            var clientSecret = _configuration["KeyVault:AuthClientSecret"];

            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, null);
            var result = context.AcquireToken(resource, clientCredential);

            return result.AccessToken;
        }
        public  Uri SetRequestUri(Uri requestUri, HttpClient httpClient)
        {
            var targetUri = requestUri;

            // NOTE: The KmsNetworkUrl setting is purely for development testing on the
            //       Microsoft Azure Development Fabric and should not be used outside that environment.
            string networkUrl = _configuration["KmsNetworkUrl"];

            if (!string.IsNullOrEmpty(networkUrl))
            {
                var authority = targetUri.Authority;
                targetUri = new Uri(new Uri(networkUrl), targetUri.PathAndQuery);

                httpClient.DefaultRequestHeaders.Add("Host", authority);
            }

            return targetUri;
        }
        public static X509Certificate2 FindCertificateByThumbprint(string findValue)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindByThumbprint, findValue, false); // Don't validate certs, since the test root isn't installed.
                if (col == null || col.Count == 0)
                    return null;
                return col[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}