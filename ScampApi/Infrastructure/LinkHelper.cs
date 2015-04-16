using System;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;

namespace ScampApi.Infrastructure
{
    public class LinkHelper : ILinkHelper
    {
        private readonly string _apiRootUrl;
        private IUrlHelper _urlHelper;

        public LinkHelper(IUrlHelper urlHelper, IConfiguration configuration)
        {
            _urlHelper = urlHelper;
            _apiRootUrl = configuration.Get("ApiRootUrl");
        }

        public string Groups()
        {
            return FullyQualify(_urlHelper.RouteUrl("Groups.GetAll"));
        }
        
        public string Group(string groupId)
        {
            return FullyQualify(_urlHelper.RouteUrl("Groups.GetSingle", new { groupId }));
        }

        public string GroupResource(string groupId, string resourceId)
        {
            return FullyQualify(_urlHelper.RouteUrl("GroupResources.GetSingle", new { groupId, resourceId }));
        }
        public string GroupTemplate(string groupId, int templateId)
        {
            return FullyQualify(_urlHelper.RouteUrl("GroupTemplates.GetSingle", new { groupId, templateId }));
        }
        public string GroupUser(string groupId, string userId)
        {
            return FullyQualify(_urlHelper.RouteUrl("GroupUsers.GetSingle", new { groupId , userId }));
        }
        public string GroupResourceUser(string groupId, string resourceId, string userId)
        {
            return FullyQualify(_urlHelper.RouteUrl("GroupResourceUsers.GetSingle", new { groupId, resourceId , userId }));
        }

        public string CurrentUser()
        {
            return FullyQualify(_urlHelper.RouteUrl("User.CurrentUser"));
        }
        public string Users()
        {
            return FullyQualify(_urlHelper.RouteUrl("Users.GetAll"));
        }

		public string NextUsers(string continuationToken)
		{
			return FullyQualify(_urlHelper.RouteUrl("Users.Next", new { continuationToken }));
		}
		public string User(string userId)
        {
            return FullyQualify(_urlHelper.RouteUrl("Users.GetSingle", new {  userId }));
        }
        private string FullyQualify(string url)
        {
            // TODo - put proper implementation here ;-)
            return _apiRootUrl + url;
        }


    }
}