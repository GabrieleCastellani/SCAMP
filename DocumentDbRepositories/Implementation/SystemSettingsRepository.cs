using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using DocumentDbRepositories;

namespace DocumentDbRepositories.Implementation
{

    public class SystemSettingsRepository
    {
        private readonly DocumentClient _client;
        private readonly DocumentCollection _collection;

        public SystemSettingsRepository(DocumentClient client, DocumentCollection collection)
        {
            _client = client;
            _collection = collection;
        }

        // get a list of system administrators
        public Task<List<ScampUser>> GetSystemAdministrators()
        {
            var admins = from u in _client.CreateDocumentQuery<ScampUser>(_collection.SelfLink)
                         where u.isSystemAdmin == true
                         select u;
            var adminList = admins.ToList();
            if (adminList.Count == 0)
                return Task.FromResult((List<ScampUser>)null);
            return Task.FromResult(adminList);
        }
    }
}
