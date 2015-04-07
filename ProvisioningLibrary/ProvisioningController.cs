using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ProvisioningLibrary
{
    public class ProvisioningController
    {
        private readonly SubscriptionCloudCredentials _credentials;
        public ProvisioningController(string certificate, string subscriptionId)
        {

            _credentials = GetCloudCredentials(certificate, subscriptionId);

        }
        private X509Certificate2 GetCertificate(string certificate)
        {
            //          The custom portal will need a management certificate to perform activities against the Azure API, and the easiest approach to obtain a management certificate is to download
            //          http://go.microsoft.com/fwlink/?LinkID=301775
            //          Inside the XML File
            //          ManagementCertificate="...base64 encoded certificate data..." />

            return new X509Certificate2(Convert.FromBase64String(certificate));
        }
        private SubscriptionCloudCredentials GetCloudCredentials(string certificate, string subscriptionId)
        {
            return new CertificateCloudCredentials(subscriptionId, GetCertificate(certificate));
        }

        public IEnumerable<string> GetVirtualMachineImagesList()
        {
            using (var computeClient = new ComputeManagementClient(_credentials))
            {
                var operatingSystemImageListResult =
                    computeClient.VirtualMachineOSImages.ListAsync().Result;



                return from image in operatingSystemImageListResult
                       select image.Name;
            }
        }
        public async Task<string> CreateVirtualMachine(string virtualMachineName, string cloudServiceName, string storageAccountName, string username, string password, string imageFilter, string virtualMachineSize, int rdpPort, bool isCloudServiceAlreadyCreated)
        {
            using (var computeClient = new ComputeManagementClient(_credentials))
            {
                // get the list of images from the api
                var operatingSystemImageListResult =
                    await computeClient.VirtualMachineOSImages.ListAsync();

                // find the one i want
                var virtualMachineOsImage = operatingSystemImageListResult
                    .Images
                    .FirstOrDefault(x =>
                        x.Name.Contains(imageFilter));


                if (virtualMachineOsImage == null)
                {
                    throw new Exception("OS Image Name Not Foud");
                }
                var imageName =
                    virtualMachineOsImage.Name;


                // set up the configuration set for the windows image
                var windowsConfigSet = new ConfigurationSet
                {
                    ConfigurationSetType = ConfigurationSetTypes.WindowsProvisioningConfiguration,
                    AdminPassword = password,
                    AdminUserName = username,
                    ComputerName = virtualMachineName,
                    HostName = string.Format("{0}.cloudapp.net", cloudServiceName)
                  
                };

                // make sure i enable powershell & rdp access
                var endpoints = new ConfigurationSet
                {
                    ConfigurationSetType = "NetworkConfiguration",
                    InputEndpoints = new List<InputEndpoint>
                    {
                        //new InputEndpoint
                        //{
                        //    Name = "PowerShell", LocalPort = 5986, Protocol = "tcp", Port = 5986,
                        //},
                        new InputEndpoint
                        {
                            Name = "Remote Desktop",
                            LocalPort = 3389,
                            Protocol = "tcp",
                            Port = rdpPort,
                        }
                    }
                };

                // set up the hard disk with the os
                var vhd = SetOsVirtualHardDisk(virtualMachineName, storageAccountName, imageName);
                // create the role for the vm in the cloud service
                var role = new Role
                {
                    RoleName = virtualMachineName,
                    RoleSize = virtualMachineSize,
                    RoleType = VirtualMachineRoleType.PersistentVMRole.ToString(),
                    OSVirtualHardDisk = vhd,
                    ProvisionGuestAgent = true,

                    ConfigurationSets = new List<ConfigurationSet>
                    {
                        windowsConfigSet,
                        endpoints
                    }
                };
                var isDeploymentCreated = false;
                if (isCloudServiceAlreadyCreated)
                {
                    var vm = await computeClient.HostedServices.GetDetailedAsync(cloudServiceName);
                    isDeploymentCreated = vm.Deployments.ToList().Any(x => x.Name == cloudServiceName);
                }

                if (isDeploymentCreated)
                {
                    AddRole(cloudServiceName, cloudServiceName, role, DeploymentSlot.Production);
                }
                else
                {
                    // create the deployment parameters
                    var createDeploymentParameters = new VirtualMachineCreateDeploymentParameters
                    {
                        Name = cloudServiceName,
                        Label = cloudServiceName,
                        DeploymentSlot = DeploymentSlot.Production,
                        Roles = new List<Role> {role}
                    };


                    // deploy the virtual machine
                    var deploymentResult = await computeClient.VirtualMachines.CreateDeploymentAsync(
                        cloudServiceName,
                        createDeploymentParameters);
                }

                // return the name of the virtual machine
                return virtualMachineName;
            }
        }

        private static OSVirtualHardDisk SetOsVirtualHardDisk(string virtualMachineName, string storageAccountName,
            string imageName)
        {
            var vhd = new OSVirtualHardDisk
            {
                SourceImageName = imageName,
                HostCaching = VirtualHardDiskHostCaching.ReadWrite,
                MediaLink = new Uri(string.Format(CultureInfo.InvariantCulture,
                    "https://{0}.blob.core.windows.net/vhds/{1}.vhd", storageAccountName, virtualMachineName),
                    UriKind.Absolute)
            };
            return vhd;
        }

        private void AddRole( string cloudServiceName, string deploymentName, Role role, DeploymentSlot slot = DeploymentSlot.Production)
        {
            try
            {
                using (var computeClient = new ComputeManagementClient(_credentials))
                {
                
                VirtualMachineCreateParameters createParams = new VirtualMachineCreateParameters
                {
                    RoleName = role.RoleName,
                    RoleSize = role.RoleSize,
                    OSVirtualHardDisk = role.OSVirtualHardDisk,
                    ConfigurationSets = role.ConfigurationSets,
                    AvailabilitySetName = role.AvailabilitySetName,
                    DataVirtualHardDisks = role.DataVirtualHardDisks,
                    ProvisionGuestAgent = role.ProvisionGuestAgent 

                };
                computeClient.VirtualMachines.Create(cloudServiceName, deploymentName, createParams);
            }
        }
            catch (CloudException e)
            {
                throw;
            }

        }

        public async Task<string> GetVirtualMachineStatus(string virtualMachineName)
        {
            List<string> VMList = new List<string>();

            using (var computeClient = new ComputeManagementClient(_credentials))
            {
                var details = await computeClient.HostedServices.GetDetailedAsync(virtualMachineName);

                return details.Deployments[0].RoleInstances[0].InstanceStatus;
            }
        }
        public async Task StartStopVirtualMachine(string virtualMachineName, string cloudServiceName, VirtualMachineAction action)
        {
            using (var computeClient = new ComputeManagementClient(_credentials))
            {
                HostedServiceGetDetailedResponse vm;
                try
                {
                    vm = await computeClient.HostedServices.GetDetailedAsync(cloudServiceName);
                }
                catch (Exception)
                {
                    Console.WriteLine("Virtual Machine not found");
                    return;
                }


                var deployment = vm.Deployments.ToList().First(x => x.Name == cloudServiceName);

                if (deployment != null)
                {
                    var deploymantSlotName = deployment.Name;
                    var serviceName = vm.ServiceName;
                    var instance = deployment.RoleInstances.First(x => x.HostName == virtualMachineName);

                    if (action == VirtualMachineAction.Start)
                    {
                        if (instance.InstanceStatus == "ReadyRole")
                        {
                            Console.WriteLine("VM Already Started");
                            return;
                        }
                        //TODO this is strange but for now i leave it a is. Need to be refactored.
                        await computeClient.VirtualMachines.StartAsync(serviceName, deploymantSlotName, instance.HostName);

                    }
                    else
                    {
                        if (instance.InstanceStatus == "StoppedVM")
                        {
                            Console.WriteLine("VM Already Stopped");
                            return;
                        }
                        computeClient.VirtualMachines.Shutdown(serviceName, deploymantSlotName, instance.HostName, new VirtualMachineShutdownParameters { });
                    }
                }
            }
        }
        public async Task<List<string>> GetVirtualMachineList()
        {
            var vmList = new List<string>();

            using (var computeClient = new ComputeManagementClient(_credentials))
            {
                var list = await computeClient.HostedServices.ListAsync();
                vmList.AddRange(list.Select(item => item.ServiceName));
            }

            return vmList;
        }
        public async Task<string> CreateStorageAccount(string location, string accountName = "")
        {
            if (accountName == "")
                accountName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            using (var storageClient = new StorageManagementClient(_credentials))
            {
                //Check if is already created
                var list = await storageClient.StorageAccounts.ListAsync();
                if (list.Any(x => x.Name == accountName)) return accountName;

                var result = await storageClient.StorageAccounts.CreateAsync(
                    new StorageAccountCreateParameters
                    {

                        AccountType = "Standard_LRS",
                        Label = "Sample Storage Account",
                        Location = location,
                        Name = accountName
                    });
            }

            return accountName;
        }

        public async Task<bool> IsCloudServiceAlreadyCreated(string cloudServiceName)
        {
            using (var computeManagementClient = new ComputeManagementClient(_credentials))
            {

                var list = await computeManagementClient.HostedServices.ListAsync();
                var check = list.HostedServices.FirstOrDefault(x => x.ServiceName == cloudServiceName);
                if (check != null)
                {
                    Console.WriteLine("Cloud Service alreaady created");
                    return true;
                }
            }
            return false;
        }
        public async Task<string> CreateCloudService(string cloudServiceName, string location)
        {
            using (var computeManagementClient = new ComputeManagementClient(_credentials))
            {
                var createHostedServiceResult = await computeManagementClient.HostedServices.CreateAsync(
                    new HostedServiceCreateParameters
                    {
                        Label = cloudServiceName + " CloudService",
                        Location = location,
                        ServiceName = cloudServiceName
                    });
                
            }

            return cloudServiceName;
        }
        internal async Task<bool> IsCloudServiceNameAvailable(string cloudServiceName)
        {
            var management = CloudContext.Clients.CreateCloudServiceManagementClient(_credentials);
            var listResult = await management.CloudServices.ListAsync();

            var search = listResult.CloudServices.First(x => x.Name == cloudServiceName);
            return search == null;
        }
        internal async Task<bool> IsStorageAccountNameAvailable(string storageAccountName)
        {
            var management = CloudContext.Clients.CreateStorageManagementClient(_credentials);
            var result = await management.StorageAccounts.CheckNameAvailabilityAsync(storageAccountName);
            return result.IsAvailable;
        }

    }
}
