namespace ScampApi.Infrastructure
{
    public interface ILinkHelper
    {
        string Groups();
        string Group(string groupId);
        string GroupResource(string groupId, string resourceId);
        string GroupTemplate(string groupId, int templateId);
        string GroupUser(string groupId, string userId);
        string CurrentUser();
        string Users();
		string NextUsers(string continuationToken);
        string User(string userId);
        string GroupResourceUser(string groupId, string resourceId, string userId);
    }
}