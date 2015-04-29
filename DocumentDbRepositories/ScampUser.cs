using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace DocumentDbRepositories
{
    public class ScampUser : Resource
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get { return "user"; } }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "isSystemAdmin")]
        public bool isSystemAdmin { get; set; }
    }

    public class ScampUserReference
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        public static implicit operator ScampUserReference(ScampUser user)
        {
            return new ScampUserReference { Id = user.Id, Name = user.Name };
        }
    }
}