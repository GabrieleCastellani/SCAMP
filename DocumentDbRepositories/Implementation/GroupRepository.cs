using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DocumentDbRepositories.Implementation
{

    public class GroupRepository
    {
        private readonly DocumentClient _client;
        private readonly DocumentCollection _collection;

        public GroupRepository(DocumentClient client, DocumentCollection collection)
        {
            _client = client;
            _collection = collection;
        }
        public Task<ScampResourceGroup> GetGroup(string groupID)
        {
            var groupQuery = from g in _client.CreateDocumentQuery<ScampResourceGroup>(_collection.SelfLink)
                             where g.Id == groupID && g.Type == "group"
                             select g;
            var group = groupQuery.ToList().FirstOrDefault();

            return Task.FromResult(group);
        }

        public Task<ScampResourceGroupWithResources> GetGroupWithResources(string groupID)
        {
            var groupQuery = from g in _client.CreateDocumentQuery<ScampResourceGroupWithResources>(_collection.SelfLink)
                             where g.Id == groupID && g.Type == "group"
                             select g;
            var group = groupQuery.ToList().FirstOrDefault();
            
            if (group == null)
                return Task.FromResult((ScampResourceGroupWithResources)null);

            var resourcesQuery = from r in _client.CreateDocumentQuery<ScampResource>(_collection.SelfLink)
                                 where r.Type == "resource" && r.ResourceGroup.Id == groupID
                                 select r;

            group.Resources = resourcesQuery.ToList();

            return Task.FromResult(group);
        }
        public Task<IEnumerable<ScampResourceGroup>> GetGroups()
        {
            var groups =
                _client.CreateDocumentQuery<ScampResourceGroup>(_collection.SelfLink)
                    .Where(u => u.Type == "group");
            var grouplist = groups.ToList();
            return Task.FromResult((IEnumerable<ScampResourceGroup>)grouplist);
        }
        public Task<IEnumerable<ScampResourceGroup>> GetGroupsByUser(ScampUserReference user)
        {
            var groups =
                _client.CreateDocumentQuery<ScampResourceGroup>(_collection.SelfLink)
                    .Where(u => u.Type == "group" && (u.Admins[0].Id == user.Id || u.Members[0].Id == user.Id));
            var grouplist = groups.ToList();
            return Task.FromResult((IEnumerable<ScampResourceGroup>)grouplist);
        }

        public async Task CreateGroup(ScampResourceGroup newGroup)
        {
            var created = await _client.CreateDocumentAsync(_collection.SelfLink, newGroup);
        }

        public Task AddResource(string groupID)
        {
            //TODO: stuff
            throw new NotImplementedException();
        }

        public Task UpdateGroup(string groupID, ScampResourceGroup group)
        {
			// TODO: Security
			Document document = _client.CreateDocumentQuery(_collection.SelfLink)
				.Where(d => d.Id == groupID)
				.AsEnumerable()
				.FirstOrDefault();
			return _client.ReplaceDocumentAsync(document.SelfLink, group);
        }
        public Task AddAdmin(string groupID)
        {
            //TODO: stuff
            throw new NotImplementedException();
        }
	}
}
