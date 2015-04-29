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
    [Route("api/settings/admins")]
    public class SettingsAdminController : Controller
    {
        private ILinkHelper _linkHelper;
        private readonly SystemSettingsRepository _settingsRepository;
        private readonly UserRepository _userRepository;
        private ISecurityHelper _securityHelper;

        public SettingsAdminController(ILinkHelper linkHelper, SystemSettingsRepository settingsRepository, UserRepository userRepository, ISecurityHelper securityHelper)
        {
            _linkHelper = linkHelper;
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
            _securityHelper = securityHelper;
        }

        // Retrieve a list of system administrators
        [HttpGet(Name = "Admin.Get")]
        public async Task<List<User>> Get()
        {
            List<User> rtnList = null;

            // fetch user from database
            List<ScampUser> adminList = await _settingsRepository.GetSystemAdministrators();

            // map data model to view model
            if (adminList != null)
            {
                rtnList = new List<User>();

                foreach (ScampUser tmpUser in adminList)
                {
                    rtnList.Add(new User
                    {
                        Id = tmpUser.Id,
                        Name = tmpUser.Name,
                        isSystemAdmin = tmpUser.isSystemAdmin
                    });
                }
            }        

            return rtnList;
        }

        // grant a user system administrator permission
        [HttpPost("{userId}", Name = "Admin.Grant")]
        public async Task grantAdmin(string userId)
        {
            // grant specified user "system admin" rights
            await updateAdmin(userId, true);
        }

        // revoke system administrator permissions for a user
        [HttpDelete("{userId}", Name = "Admin.Revoke")]
        public async Task revokeAdmin(string userId)
        {
            //TODO: check for last remaining admin
            List<User> userList = await Get();
            if (userList.Count <= 1)
                throw new InvalidOperationException("This would remove the last system admin. There must always be at least 1.");

            // revoke admin rights
            await updateAdmin(userId, false);
        }

        private async Task updateAdmin(string userId, bool isAdmin)
        {
            // ensure requestor has system admin permissions
            if (!await _securityHelper.IsSysAdmin())
                throw new AccessViolationException("Access Denied, requestor is not a system administrator");
            
            //TODO: need exception handling

            ScampUser user = await _userRepository.GetUserbyId(userId);
            
            //TODO: make sure we got a user and throw a 404 if not 

            // only perform update if value needs to be changed
            if (user.isSystemAdmin != isAdmin)
            {
                // if not, grant them permission
                user.isSystemAdmin = isAdmin;

                // save updated setting
                await _userRepository.UpdateUser(user);
            }
        }
    }
}
