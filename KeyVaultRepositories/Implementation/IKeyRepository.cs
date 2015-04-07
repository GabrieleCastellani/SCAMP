namespace KeyVaultRepositories.Implementation
{
    public interface IKeyRepository
    {
        string  GetSecret(string resourceId, string key);
        bool UpsertSecret(string resourceId, string key, string value);
        bool DeleteSecret(string resourceId, string key);
    }
}