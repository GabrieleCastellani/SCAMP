using System.Threading.Tasks;
using DocumentDbRepositories;

namespace ScampApi.Infrastructure
{
    public interface ISecurityHelper
    {
        string GetIPIDByContext();
        Task<ScampUser> GetUser();
        Task<ScampUser> GetUserByIPID(string IPID);
        Task<ScampUserReference> GetUserReference();
        Task<bool> IsGroupAdmin(string groupId);
        Task<bool> IsSysAdmin();
    }
}