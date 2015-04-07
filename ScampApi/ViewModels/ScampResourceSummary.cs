using System.Collections.Generic;
using DocumentDbRepositories;
using Newtonsoft.Json;

namespace ScampApi.ViewModels
{
    public class ScampResourceSummary
    {
        public ScampResourceSummary()
        {
            Links = new List<Link>();
        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "resourceGroup")]
        public ScampResourceGroupReference ResourceGroup { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "remaining")]
        public int Remaining { get; set; }

        public List<Link> Links { get; set; }

    }
}
