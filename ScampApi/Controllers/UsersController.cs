using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using ScampApi.Infrastructure;
using ScampApi.ViewModels;
using DocumentDbRepositories.Implementation;
using DocumentDbRepositories;
using System.Threading.Tasks;

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
			//LINKED TO UI
			return from u in await _userRepository.GetUsers()
			select map(u);

            //return new[] {
            //    new UserSummary {
            //        UserId ="1",
            //        Name = "User1",
            //        Links = {
            //             new Link {Rel="user", Href= _linkHelper.User("1") }
            //        }
            //    },
            //     new UserSummary {
            //        UserId ="2",
            //        Name = "User2",
            //        Links = {
            //             new Link {Rel="user", Href= _linkHelper.User("2") }
            //        }
            //    }
            //};
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

        // POST api/values
        [HttpPost("{userId}")]
        public void Post(int userId,[FromBody]string value)
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

		private UserSummary map(ScampUser docDbUSer)
		{
			return new UserSummary
			{
				UserId = docDbUSer.Id,
				Name = docDbUSer.Name
			};
		}
	}
}
