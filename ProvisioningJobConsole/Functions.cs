using System;
using System.Diagnostics;
using System.IO;
using DocumentDbRepositories;
using DocumentDbRepositories.Implementation;
using KeyVaultRepositories.Implementation;
using Microsoft.Azure.WebJobs;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.WindowsAzure.Management.Compute.Models;
using ProvisioningLibrary;
using ProvisioningLibrary5x;

namespace ProvisioningJobConsole
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public async static void ProcessQueueMessage([QueueTrigger("processorqueue")] QueueMessage message, TextWriter log)
        {

            // Setup configuration sources.
            var configuration = new Configuration()
                 .AddEnvironmentVariables("APPSETTING_");
            var services = new ServiceCollection();
            services.AddDocumentDbRepositories(configuration);
            services.AddKeyVaultRepositories(configuration);
            services.AddTransient<ResourceController>();
            
            var serviceProvider = services.BuildServiceProvider();



            // TODO - shouldn't be depending on ResourceController here
            var keyVaultController = serviceProvider.GetService<IKeyRepository>();
            var resourceController = serviceProvider.GetService<ResourceController>();
            
            var docDbResource = await resourceController.GetResource(message.ResourceId);
            ScampSubscription subscription;
            string cloudServiceName, machineName;

            if (docDbResource == null)
            {
                Console.WriteLine("Resource not found");
                return;
            }


            if (string.IsNullOrEmpty(docDbResource.SubscriptionId))
            {
                //need to create a VM
                Console.WriteLine("Creating VM");

                subscription = await resourceController.GetAvailabeDeploymentSubscription();
                message.Action = ResourceAction.Create;
                cloudServiceName = await resourceController.GetCloudServiceName(docDbResource);
                machineName = docDbResource.Name;
            }
            else
            {
                subscription = await resourceController.GetSubscription(docDbResource.SubscriptionId);
                machineName = docDbResource.Name;
                cloudServiceName = docDbResource.CloudServiceName;
            }


            var provisioningController = new ProvisioningController(subscription.AzureManagementThumbnail, subscription.AzureSubscriptionID);

       
            //TODO Get Connection string from DB
            if (message.Action == ResourceAction.Delete)
            {
                //TODO Temporary
                try
                {
                    await provisioningController.StartStopVirtualMachine(machineName, cloudServiceName, VirtualMachineAction.Stop);
                }
                catch (Exception e)
                {
                }

                await resourceController.DeleteResource(docDbResource);
            }
            if (message.Action == ResourceAction.Stop)
            {
                Console.WriteLine("Stopping VM");
                await provisioningController.StartStopVirtualMachine(machineName, cloudServiceName, VirtualMachineAction.Stop);
                docDbResource.State = "Stopped";
                await resourceController.UpdateResource(docDbResource);
            }
            if (message.Action == ResourceAction.Start)
            {
                Console.WriteLine("Starting VM");
                var x = provisioningController.StartStopVirtualMachine(machineName, cloudServiceName, VirtualMachineAction.Start);
                x.Wait();
                docDbResource.State = "Started";
                await resourceController.UpdateResource(docDbResource);
            }
            if (message.Action == ResourceAction.Create)
            {
                var r = new Random();
                //Need to find a way to assign Usename and Pwd
                string username =  "ScampAdmin";
                string password = "Enter.321";
                string location = resourceController.GetServiceLocation();
                string storageAccountName = cloudServiceName;
                int rdpPort = r.Next(3000, 4000);

                docDbResource.State = "Creating base services";
                await resourceController.UpdateResource(docDbResource);

                var isCloudServiceAlreadyCreated =
                                await provisioningController.IsCloudServiceAlreadyCreated(cloudServiceName);

                if (!isCloudServiceAlreadyCreated)
                {
                    Console.WriteLine("Creating Cloud Services");
                    docDbResource.State = "Creating Cloud Services";
                    await resourceController.UpdateResource(docDbResource);
                    var x = provisioningController.CreateCloudService(cloudServiceName, location);
                    x.Wait();
                    Console.WriteLine("Creating Storage");
                   
                    docDbResource.State = "Creating Storage";
                    await resourceController.UpdateResource(docDbResource);

                    await provisioningController.CreateStorageAccount(location, storageAccountName);
                }
                //TODO Need to delete disk in case is already there
                Console.WriteLine("Creating Virtual Machine");

                docDbResource.State = "Creating Virtual Machine";
                await resourceController.UpdateResource(docDbResource);

                var z = await provisioningController.CreateVirtualMachine(machineName, cloudServiceName, storageAccountName,
                    username, password, "Visual-Studio-2015-Ultimate", VirtualMachineRoleSize.Small, rdpPort, isCloudServiceAlreadyCreated);
                
                docDbResource.CloudServiceName = cloudServiceName;
                docDbResource.State = "Created - Starting";
                docDbResource.SubscriptionId = subscription.Id;
                docDbResource.UserName = username;
                //docDbResource.UserPassword = password;
                docDbResource.RdpPort = rdpPort.ToString();
                keyVaultController.UpsertSecret(docDbResource.Id, "password", password);

                await resourceController.UpdateResource(docDbResource);
            }


            Console.WriteLine(message);
        }

    }

}
