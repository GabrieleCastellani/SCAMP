using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using ScampApi.Infrastructure;
using ScampApi.ViewModels;
using DocumentDbRepositories.Implementation;
using DocumentDbRepositories;
using System.Threading.Tasks;
using System.Text;

using System.Net.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ScampApi.Controllers.Controllers
{
    [Route("api/users")]
    public class UsersController : Controller
    {
        private ILinkHelper _linkHelper;
		private UserRepository _userRepository;

        public UsersController(ILinkHelper linkHelper, UserRepository userRepository)
        {
            _linkHelper = linkHelper;
			_userRepository = userRepository;
        }
        [HttpGet(Name = "Users.GetAll")]
        public async Task<IEnumerable<UserSummary>> Get()
        {
			return from u in await _userRepository.GetUsers()
				   select AutoMapper.Mapper.Map<UserSummary>(u);

			//ASCIIEncoding.ASCII.GetString(Context.Session["usersContinuationToken"]);
			//var list = new List<UserSummary>();
			//Tuple<IEnumerable<ScampUser>, string> result = null;

			//         do
			//{
			//	result = await _userRepository.GetUsers(2, result?.Item2);

			//	var tempList = (from u in result.Item1
			//					select AutoMapper.Mapper.Map<UserSummary>(u)).ToList();
			//             list.AddRange(tempList);
			//}
			//while (result.Item2 != null);

			//LINKED TO UI

			//string userContinuationToken = null;
			//if (Context.Session["usersContinuationToken"] != null)
			//	userContinuationToken = ASCIIEncoding.ASCII.GetString(Context.Session["usersContinuationToken"]);

			//Tuple<IEnumerable<ScampUser>, string> result = result = await _userRepository.GetUsers(2, userContinuationToken);

			//if (!String.IsNullOrEmpty(result.Item2))
			//	Context.Session["usersContinuationToken"] = Encoding.ASCII.GetBytes(result.Item2);
			//else // there aren't other users
			//	Context.Session["usersContinuationToken"] = null;

			//return (from u in result.Item1
			//		select AutoMapper.Mapper.Map<UserSummary>(u)).ToList();
		}

		[HttpGet("next/{contToken}", Name = "Users.Next")]
		public async Task<Tuple<IEnumerable<UserSummary>, string>> GetNext(string contToken)
		{
			Tuple<IEnumerable<ScampUser>, string> result = await _userRepository.GetUsers(3, Uri.UnescapeDataString(contToken));

			string encoded = null;
            if (result.Item2 != null)
				encoded = Uri.EscapeDataString(result.Item2);

            return new Tuple<IEnumerable<UserSummary>, string>
			(
				(from u in result.Item1 select AutoMapper.Mapper.Map<UserSummary>(u)).ToList(),
				encoded
			);
        }

        [HttpGet("{userId}", Name = "Users.GetSingle")]
        public User Get(string userId)
        {
            return new User
            {
                Id = "1",
                Name = "User1",
                Groups = new[] { new GroupSummary { GroupId = "Id1", Name = "Group1", Links = { new Link { Rel = "group", Href = _linkHelper.Group(groupId: "Id1") } } } },
                //Resources = new[] { new ScampResourceSummary { GroupId = "Id1", ResourceId = "1", Name = "GroupResource1", Links = { new Link { Rel = "groupResource", Href = _linkHelper.GroupResource(groupId: "Id1", resourceId: "1") } } } }
            };
        }

        [HttpGet("byname/{searchparm}", Name = "Users.SearchByName")]
        public User GetByName(string searchparm)
        {
            throw new NotImplementedException();
        }


        // POST api/values
        [HttpPost("{userId}")]
		public void Post(int userId, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/values/5
        [HttpPut("{userId}")]
        public void Put(int userId, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/values/5
        [HttpDelete("{userId}")]
        public void Delete(int userId)
        {
            throw new NotImplementedException();
        }
	}
}
