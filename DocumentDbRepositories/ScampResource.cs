using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentDbRepositories
{
    public class ScampResource
    {
        public ScampResource()
        {
            Owners = new List<ScampUserReference>();
        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "resourceGroup")]
        public ScampResourceGroupReference ResourceGroup { get; set; }
        [JsonProperty(PropertyName = "azureResourceId")]
        public string AzureResourceId { get; set; }
        [JsonProperty(PropertyName = "subscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonProperty(PropertyName = "resourceType")]
        public string ResourceType { get; set; }
        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }
        [JsonProperty(PropertyName = "cloudServiceName")]
        public string CloudServiceName { get; set; }
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "userPassword")]
        public string UserPassword { get; set; }
        [JsonProperty(PropertyName = "rdpPort")]
        public string RdpPort { get; set; }

        [JsonProperty(PropertyName = "owners")]
        public List<ScampUserReference> Owners { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get { return "resource"; } }

    }
}
