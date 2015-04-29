using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDbRepositories;
using DocumentDbRepositories.Implementation;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace ScampApi.Infrastructure
{
    
    public class SecurityHelper : ISecurityHelper
    {
        private readonly HttpContext Context;
        private readonly UserRepository _userRepository;
        private readonly GroupRepository _groupRepository;

        public SecurityHelper(IHttpContextAccessor httpContextAccessor, UserRepository userRepository, GroupRepository groupRepository )
        {
            Context = httpContextAccessor.Value;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
        }

        public async Task<ScampUserReference> GetUserReference()
        {
            return  Mapper.Map<ScampUserReference>(await GetUser());
        } 
        public async Task<ScampUser> GetUser()
        {
            //TODO ADD Some cache
            var userId = GetIPIDByContext();
            return await GetUserById(userId);
        }

        public string GetIPIDByContext()
        {
            // get Tenant and Object ID claims
            string tenantID = Context.User.Claims.FirstOrDefault(c => c.Type.Contains("tenantid")).Value;
            string objectID = Context.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier")).Value;
            // create SCAMP UserID
            string IPID = string.Format("{0}-{1}", tenantID, objectID);
            return IPID;
        }

        public async Task<ScampUser> GetUserById(string userId)
        {
            var tmpUser = await _userRepository.GetUserbyId(userId);
            if (tmpUser == null) // insert if user doesn't exist
            {
                // build user object
                tmpUser = new ScampUser()
                {
                    Id = userId,
                    Name =
                        string.Format("{0} {1}", Context.User.FindFirst(ClaimTypes.GivenName).Value,
                            Context.User.FindFirst(ClaimTypes.Surname).Value).Trim(),
					//isSystemAdmin = tmpUser.isSystemAdmin,
					isSystemAdmin = true,
					// get email address
					email = Context.User.Claims.FirstOrDefault(c => c.Type.Contains("email") || c.Type.Contains("upn")).Value
                };

                // insert into database   
                await _userRepository.CreateUser(tmpUser);
            }
            return tmpUser;
        }
        public async Task<bool> IsGroupAdmin(string groupId)
        {
            var user = await GetUser();
            var grp = await _groupRepository.GetGroup(groupId);
            var checkAdmin = grp.Admins.ToList().Any(q => q.Id == user.Id);
            return checkAdmin;
        }

        public async Task<bool> IsSysAdmin()
        {
            ScampUser user = await GetUser();

            return user.isSystemAdmin;
        }
    }
}