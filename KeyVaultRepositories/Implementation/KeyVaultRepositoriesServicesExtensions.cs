﻿using System;
using System.IO.Pipes;
using System.Net.Http;
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

       public KeyVaultScampClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _keyVaultUrl = _configuration["KeyVault:Url"];
            _keyVaultClient = new KeyVaultClient(GetAccessToken, setRequestUriCallback: SetRequestUri);
        }

       public KeyVaultClient GetClient()
       {
           return _keyVaultClient;
       }
        public string  GetKeyVaultUrl()
        {
            return _keyVaultUrl;
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
    }
}