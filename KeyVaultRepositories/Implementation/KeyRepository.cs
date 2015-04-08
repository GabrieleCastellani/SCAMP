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
        private static string SecretKey(string resourceId, string key)
        {
            var secretKey = string.Format("{0}-{1}", resourceId, key);
            return secretKey;
        }
        public string  GetSecret(string resourceId, string key)
        {
            //TODO Add some caching
            var secretKey = SecretKey(resourceId, key);
            var client = _keyVaultClient.GetClient();
            var secret= client.GetSecretAsync(_keyVaultClient.GetKeyVaultUrl(), secretKey).GetAwaiter().GetResult();
            return secret.SecureValue.ConvertToString();
        }
        public bool UpsertSecret(string resourceId, string key, string value)
        {
            var secretKey = SecretKey(resourceId, key);
            var client=_keyVaultClient.GetClient();
            var secret = client.SetSecretAsync(_keyVaultClient.GetKeyVaultUrl(), secretKey, value.ConvertToSecureString()).GetAwaiter().GetResult();

            return true;
        }



        public bool DeleteSecret(string resourceId, string key)
        {
            var secretKey = SecretKey(resourceId, key);
            var client = _keyVaultClient.GetClient();
            var secret = client.DeleteSecretAsync(_keyVaultClient.GetKeyVaultUrl() , secretKey).GetAwaiter().GetResult();
            return true;
        }
    }
}