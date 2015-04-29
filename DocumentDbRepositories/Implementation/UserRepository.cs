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
    public class UserRepository 
    {
        private readonly DocumentClient _client;
        private readonly DocumentCollection _collection;

        public UserRepository(DocumentClient client, DocumentCollection collection)
        {
            _client = client;
            _collection = collection;
        }
        public async Task CreateUser(ScampUser newUser)
		{
			var created = await _client.CreateDocumentAsync(_collection.SelfLink, newUser);
		}

		public Task<ScampUser> GetUserbyId(string userId)
		{
            // get specified user by ID
            var queryResult = from u in _client.CreateDocumentQuery<ScampUser>(_collection.SelfLink)
                              where u.Id == userId
						select u;
            var user = queryResult.ToList().FirstOrDefault();

            if (user == null)
                return Task.FromResult((ScampUser)null);

            return Task.FromResult((ScampUser)user);
		}

		public Task<IEnumerable<ScampUser>> GetUsers()
		{
			var users = from u in _client.CreateDocumentQuery<ScampUser>(_collection.SelfLink)
						where u.Type == "user"
						select u;

			return Task.FromResult((IEnumerable<ScampUser>)users.ToList());
		}

		public async Task<Tuple<IEnumerable<ScampUser>, string>> GetUsers(int maxItems, string continuationToken)
		{

			FeedOptions feedOptions = new FeedOptions()
			{
				MaxItemCount = maxItems,
				RequestContinuation = continuationToken
			};

			IDocumentQuery<ScampUser> query =
				(from u in _client.CreateDocumentQuery<ScampUser>(_collection.SelfLink, feedOptions)
				 where u.Type == "user"
				 select u).AsDocumentQuery();

			FeedResponse<ScampUser> pagedUsers = await query.ExecuteNextAsync<ScampUser>();

			return new Tuple<IEnumerable<ScampUser>, string>((IEnumerable<ScampUser>)pagedUsers.ToList(), pagedUsers.ResponseContinuation);
		}

		public async Task UpdateUser(ScampUser user)
        {
            //TODO: likely need to do more here

            //ScampUser tmpUser = (dynamic)userDoc;
            user.isSystemAdmin = false;
            var savedUser = await _client.ReplaceDocumentAsync(user.SelfLink, user);

            // exception handling, etc... 

        }
    }
}
