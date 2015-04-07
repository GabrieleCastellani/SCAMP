using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Framework.ConfigurationModel;
using ProvisioningLibrary;
using ProvisioningJobConsole;

namespace ProvisioningJobConsole
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        static void Main()
        {
            // Setup configuration sources.
            Configuration = new Configuration()
                .AddEnvironmentVariables("APPSETTING_");


        

            var storageCstr = GetConnectionString();

            JobHostConfiguration config = new JobHostConfiguration(storageCstr);
            config.Queues.BatchSize = 1; //Number of messages parallel processed in parallel. Will need some concurrency check before increasing.
            config.Queues.MaxDequeueCount = 4;
            config.Queues.MaxPollingInterval = TimeSpan.FromSeconds(15);
            JobHost host = new JobHost(config);

            
            // TEST Lines
            //IDictionary<string, string> settings = new Dictionary<string, string>();
            //settings.Add("Provisioning:StorageConnectionString", storageCstr);
            //ProvisioningLibrary.WebJobController w = new WebJobController(settings);
            //w.SubmitActionInQueue("ee2a86c8-3873-42be-91fc-2f5ec95a8477", ResourceAction.Start );


            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        private static string GetConnectionString()
        {

            var rv = Configuration["Provisioning:StorageConnectionString"];


            if (string.IsNullOrEmpty(rv))
            {
                throw new ArgumentNullException(@"you're missing StorageConnectionString in either ENV or Config");

            }


            return rv;
        }
    }
}
