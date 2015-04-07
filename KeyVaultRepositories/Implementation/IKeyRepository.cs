namespace KeyVaultRepositories.Implementation
{
    public interface IKeyRepository
    {
        string  GetSecret(string secretName);
        bool UpsertSecret(string secretName, string secretValue);
        bool DeleteSecret(string secretName);
    }
}