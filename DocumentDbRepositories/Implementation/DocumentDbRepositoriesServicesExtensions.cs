using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace DocumentDbRepositories.Implementation
{
    public static class DocumentDbRepositoriesServicesExtensions
    {
        public static void AddDocumentDbRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            DocumentClient client;
            DocumentCollection collection;

            GetClientAndCollection(configuration, out client, out collection);

            // Sharing instance of DocumentClient as per "SDK Usage Tip #1: Use a singleton DocumentDB client for the lifetime of your application":
            // http://azure.microsoft.com/blog/2015/01/20/performance-tips-for-azure-documentdb-part-1-2/
            services.AddInstance(client);
            services.AddInstance(collection);

            services.AddTransient<GroupRepository>();
            services.AddTransient<ResourceRepository>();
            services.AddTransient<SubscriptionRepository>();
            services.AddTransient<UserRepository>();
            services.AddTransient<SystemSettingsRepository>();
        }

        // TODO think about whether there are alternative ways to deal with the mis-match between async initialization and the sync services pipeline
        private static void GetClientAndCollection(IConfiguration configuration, out DocumentClient client, out DocumentCollection collection)
        {
            var endpoint = configuration["DocDb:endpoint"];
            var authKey = configuration["DocDb:authkey"];
            var databaseName = configuration["DocDb:databaseName"];
            var collectionName = configuration["DocDb:collectionName"];
            var connectionMode = configuration["DocDb:ConnectionMode"];

            var connectionPolicy = GetConnectionPolicy(connectionMode);


            client = new DocumentClient(new Uri(endpoint), authKey, connectionPolicy);
            var database = GetOrCreateDatabaseAsync(client, databaseName).Result;

            //Get, or Create, the Document Collection
            collection = GetOrCreateCollectionAsync(client, database.SelfLink, collectionName).Result;
        }

        private static ConnectionPolicy GetConnectionPolicy(string connectionMode)
        {
            connectionMode = connectionMode.ToLower();
            if (connectionMode == "http")
            {
                return new ConnectionPolicy()
                {
                    ConnectionProtocol = Protocol.Https,
                    ConnectionMode = ConnectionMode.Gateway
                };
            }
            else if (connectionMode == "tcp")
            {
                return new ConnectionPolicy()
                {
                    ConnectionProtocol = Protocol.Tcp,
                    ConnectionMode = ConnectionMode.Direct
                };
            }
            else
            {
                throw new ArgumentOutOfRangeException("ConnectionMode setting must be 'http' or 'tcp'");
            }
        }

        private static async Task<Database> GetOrCreateDatabaseAsync(DocumentClient client, string id)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }
            return database;
        }

        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(DocumentClient client, string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }

            return collection;
        }
    }
}
