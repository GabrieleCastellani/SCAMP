using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentDbRepositories;
using DocumentDbRepositories.Implementation;
using Microsoft.AspNet.Mvc;
using ProvisioningLibrary;
using ScampApi.Infrastructure;
using ScampApi.ViewModels;

namespace ScampApi.Controllers
{
    [Authorize]
    [Route("api/groups/{groupId}/resources")]
    public class GroupResourcesController : Controller
    {
        private ILinkHelper _linkHelper;
        private ResourceRepository _resourceRepository;
        private ISecurityHelper _securityHelper;
        private GroupRepository _groupRepository;
        private IWebJobController _webJobController;

        public GroupResourcesController(ILinkHelper linkHelper,ISecurityHelper securityHelper, ResourceRepository resourceRepository, GroupRepository groupRepository, IWebJobController webJobController)
        {
            _linkHelper = linkHelper;
            _resourceRepository = resourceRepository;
            _securityHelper = securityHelper;
            _groupRepository = groupRepository;
            _webJobController = webJobController;
        }
        [HttpGet]
        public async Task< IEnumerable<ScampResourceSummary>> GetAll(string groupId)
        {
            //LINKED TO UI
            var res =await   _resourceRepository.GetResourcesByGroup(await _securityHelper.GetUserReference(), groupId);

            var ressummary = res.Select(Mapper.Map<ScampResourceSummary>).ToList();
            var rnd = new Random();
            ressummary.ForEach(summary =>
            {
                summary.Links.Add(new Link
                {
                    Rel = "resource",
                    Href = _linkHelper.GroupResource(summary.ResourceGroup.Id, summary.Id)
                });
                summary.Remaining = rnd.Next(0, 100);
            });

            return ressummary;
 
        }

        [HttpGet("{resourceId}", Name ="GroupResources.GetSingle")]
        public GroupResource Get(string groupId, string resourceId)
        {
            return new GroupResource
            {
                GroupId = groupId,
                ResourceId = resourceId,
                Name = "GroupResource" + resourceId,
                Users = new[]
                {
                    new UserSummary { UserId = "1", Name = "User1", Links =
                        {
                            new Link {Rel="user", Href = _linkHelper.User(userId: "1") } ,
                            new Link {Rel="groupResourceUser", Href = _linkHelper.GroupResourceUser(groupId: groupId, resourceId:resourceId, userId: "1") }
                        }
                    }
                }
            };
        }
        [HttpPost("{resourceId}/{actionname}")]
        public async void Post(string groupId, string resourceId, string actionname)
        {
            var checkPermission = await CanStartStopResource(resourceId);
            if (checkPermission)
            { 
                _webJobController.SubmitActionInQueue(resourceId, actionname);
            }
        }
        [HttpPost]
        public async Task<ScampResourceSummary>  Post(string groupId, [FromBody]ScampResourceSummary groupResource)
        {
            //LINKED TO UI
            var grp = await _groupRepository.GetGroup(groupId);

            var checkPermission =await  CanCreateResource(groupId);
            if (!checkPermission) return null;
            var grpRef = new ScampResourceGroupReference() {Id = grp.Id};
            var res = new ScampResource()
            {
                Id = Guid.NewGuid().ToString("d"),
                ResourceGroup = grpRef,
                Name = Regex.Replace(groupResource.Name.ToLowerInvariant(), "[^a-zA-Z0-9]", ""),
                ResourceType = "Virtual Machine",
                State = "Not provisioned - Need to be started"
            };

            await _resourceRepository.CreateResource(res);
            return Mapper.Map<ScampResourceSummary>(res);

        }

        [HttpPut("{resourceId}")]
        public void Put(int groupId, string resourceId, [FromBody]GroupResource value)
        {
            // TODO implement updating a group resource
            throw new NotImplementedException();
        }

        [HttpDelete("{resourceId}")]
        public async Task Delete(string  groupId, string resourceId)
        {
            //LINKED TO UI
            var checkPermission = await CanDeleteResource(groupId, resourceId);
            if(checkPermission)
            {
                var res =await  _resourceRepository.GetResource(resourceId);
                res.State = "Deleting";
                await  _resourceRepository.UpdateResource( res);
                _webJobController.SubmitActionInQueue(resourceId,ResourceAction.Delete );
            }
        }



        private async Task<bool> CanCreateResource(string groupId)
        {
            if (await _securityHelper.IsSysAdmin()) return true; //Sysadmin can do everything

            //Check if User is a Group Owner
            if (await _securityHelper.IsGroupAdmin(groupId)) return true;

            return false;
        }

        private async Task<bool> CanStartStopResource( string resourceId)
        {
            if (await _securityHelper.IsSysAdmin()) return true; //Sysadmin can do everything

            //TODO Check if User is a Resource Owner
            //TODO Check if User is a Group Owner

            return true;
        }

        private async Task<bool> CanDeleteResource(string groupId, string resourceId)
        {
            if (await _securityHelper.IsSysAdmin()) return true; //Sysadmin can do everything
            //TODO Check if resource belongs to this groupId

            //Only group admin can Delete resources

            var checkAdmin = await _securityHelper.IsGroupAdmin(groupId);
            return checkAdmin;
        }


    }
}