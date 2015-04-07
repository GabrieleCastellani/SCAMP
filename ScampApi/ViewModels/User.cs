using System;
using System.Collections.Generic;


namespace ScampApi.ViewModels
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string email { get; set; }

        public string IPKey { get; set; }

        public bool IsSystemAdmin { get; set; }
        public IEnumerable<GroupSummary> Groups { get; internal set; }
        public IEnumerable<ScampResourceSummary> Resources { get; internal set; }
    }
}