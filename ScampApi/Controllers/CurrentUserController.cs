using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using ScampApi.Infrastructure;
using ScampApi.ViewModels;
using System.Security.Claims;
using DocumentDbRepositories;
using DocumentDbRepositories.Implementation;
using System.Threading.Tasks;

namespace ScampApi.Controllers.Controllers
{
    [Authorize]
    [Route("api/currentUser")]
    public class CurrentUserController : Controller
    {
        private ILinkHelper _linkHelper;
        private readonly UserRepository _userRepository;
        private ISecurityHelper _securityHelper;

        public CurrentUserController(ILinkHelper linkHelper,ISecurityHelper securityHelper, UserRepository userRepository)
        {
            _linkHelper = linkHelper;
            _userRepository = userRepository;
            _securityHelper = securityHelper;
        }
        // GET: api/currentUser
        [HttpGet(Name = "User.CurrentUser")]
        public async Task<User> Get()
        {
            ScampUser tmpUser = null; 

            // fetch user from database
            tmpUser = await _securityHelper.GetUser();

			//TODO: we're going to need this for authorizing requests, so we should probably cache it
			//return object for user...

			return new User
			{
				Id = tmpUser.Id,
				Name = tmpUser.Name,
				isSystemAdmin = tmpUser.isSystemAdmin,
				email = tmpUser.email
            };
        }

       
    }
}
