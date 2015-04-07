using System.Collections.Generic;

namespace ScampApi.ViewModels
{
    public class Group
    {
        public Group()
        {
                Admins = new List<UserSummary>();
        }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ScampResourceSummary> Resources { get; set; }
        public IEnumerable<GroupTemplateSummary> Templates { get; set; }
        public List<UserSummary> Admins { get; set; }
        public List<UserSummary> Members { get; set; }
    }
}
