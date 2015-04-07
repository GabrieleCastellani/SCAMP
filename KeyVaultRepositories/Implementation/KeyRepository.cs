using System;
using Microsoft.KeyVault.Client;

namespace KeyVaultRepositories.Implementation
{
    public class KeyRepository : IKeyRepository
    {
        private readonly KeyVaultScampClient _keyVaultClient;

        public KeyRepository(KeyVaultScampClient keyVaultClient)
        {
            _keyVaultClient = keyVaultClient;
        }

        public string  GetSecret(string secretName)
        {
            return "";
        }
        public bool UpsertSecret(string secretName, string secretValue)
        {
            var secret = _keyVaultClient.GetClient().SetSecretAsync(_keyVaultClient.GetKeyVaultUrl(), secretName, secretValue.ConvertToSecureString()).GetAwaiter().GetResult();

            return true;
        }
        public bool DeleteSecret(string secretName)
        {
            return true;
        }
    }
}