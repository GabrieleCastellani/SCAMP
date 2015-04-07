using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace ProvisioningLibrary
{
    public class WebJobController : IWebJobController
    {
        IDictionary<string, string> _settings;

        public WebJobController(IConfiguration config)
        {
            //TODO Refactor
            IDictionary<string, string> settings = new Dictionary<string, string>();
            var storageCstr = config.Get("Provisioning:StorageConnectionString");
            settings.Add("Provisioning:StorageConnectionString", storageCstr);
            _settings = settings;   
        }

        public Guid SubmitActionInQueue(string  resourceId, ProvisioningLibrary.ResourceAction  action)
        {
            var storageConnectionString = _settings["Provisioning:StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString); 


            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("processorqueue");


            queue.CreateIfNotExists();
            var actionId = Guid.NewGuid();

            var actionMessage    = new QueueMessage()
            {
                OperationGuid = actionId,
                Action  = action,
                ResourceId = resourceId
            };


            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(actionMessage)));
            return actionId;
        }

        public void SubmitActionInQueue(string resourceId, string actionname)
        {
            actionname = actionname.ToLowerInvariant();
            var action= ResourceAction.Undefined;
            if (actionname == "start")
            {
                action = ResourceAction.Start;
            }
            if (actionname == "stop")
            {
                action = ResourceAction.Stop;
            }
            if (actionname == "create")
            {
                action = ResourceAction.Create;
            }
            if (actionname == "delete")
            {
                action = ResourceAction.Delete;
            }
            if (action == ResourceAction.Undefined) throw new Exception("Action no defined");

            this.SubmitActionInQueue(resourceId, action);

        }
    }
}
