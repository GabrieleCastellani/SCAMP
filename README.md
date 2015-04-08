# Simple Cloud Manager Project - Web Application #
This project contains a single Web App that contains both the Views/Pages along with the Web API in support of the Pages.

### Setting Azure Active Directory Tenant ###

You an use an existing Azure Active Directory AAD tenant (all Azure subscriptions have one associated with it). Or create a new AAD tenant in the [Portal](https://manage.windowsazure.com).  See [http://www.windowsazure.com](http://www.windowsazure.com).  

### Step 1:  Clone or download this repository

From your shell or command line:

	`git clone https://github.com/SimpleCloudManagerProject/Scamp-API`

### Step 2:  Register the Scamp Web App on your Azure Active Directory tenant

1. Sign in to the [Azure management portal](https://manage.windowsazure.com).
2. Click on Active Directory in the left hand nav.
3. Click the directory tenant where you wish to register the sample application.
4. Click the Applications tab.
5. In the drawer, click Add.
6. Click "Add an application my organization is developing".
7. Enter a friendly name for the application, for example "Scamp", select "Web Application and/or Web API", and click next.
8. For the sign-on URL, enter the base URL for the sample, `https://localhost:44300/`. We'll use this later.
9. For the App ID URI, enter `https://<your_tenant_name>/Scamp`, replacing `<your_tenant_name>` with the name of your Azure AD tenant.  Save the configuration.
10. Also, get the TenantID from the URL in your browser. eg. below in the URL you'll see a guid which is your Tenant ID.
```
../ActiveDirectoryExtension/Directory/<tenantId>/directoryQuickStart
```
11. Also, get the ClientID from the Application settings.  On the Application -> Configure tab, grap the "CLIENT ID" value.

### Step 3:  Configure the Scamp Web App to use your Azure Active Directory tenant
TODO

### Step 4: Create a KeyVault repository
For this step you will need Azure PowerShell version 0.8.13 or later.
You can also read the following tutorials to get familiar with Azure Resource Manager in Windows PowerShell:

- [How to install and configure Azure PowerShell](powershell-install-configure.md)
- [Using Windows PowerShell with Resource Manager](powershell-azure-resource-manager.md)

Start an Azure PowerShell session and sign in to your Azure account with the following command:  

    Add-AzureAccount

In the pop-up browser window, enter your Azure account user name and password. Windows PowerShell will get all the subscriptions that are associated with this account and by default, uses the first one.

If you have multiple subscriptions and want to specify a specific one to use for Azure Key Vault, type the following to see the subscriptions for your account:

    Get-AzureSubscription

Then, to specify the subscription to use, type:

    Select-AzureSubscription -SubscriptionName <subscription name>

If you haven't already done so, [download the scripts](https://gallery.technet.microsoft.com/scriptcenter/Azure-Key-Vault-Powershell-1349b091) and unblock the "Azure Key Vault Powershell scripts.zip" file by right-clicking it, **Properties**, **Unblock**. Then extract the zip file to a local folder on your computer.

Before you load the script module into your Azure PowerShell session, set the execution policy:

			Set-ExecutionPolicy RemoteSigned -Scope Process

Then load the script module into your Azure PowerShell session. For example, if you extracted the scripts to a folder named C:\KeyVaultScripts, type:

			import-module C:\KeyVaultScripts\KeyVaultManager

The Key Vault cmdlets and scripts require Azure Resource Manager, so type the following to switch to Azure Resource Manager mode:

	Switch-AzureMode AzureResourceManager

	New-AzureKeyVault -VaultName 'ScampKeyVault' -ResourceGroupName 'ScampResourceGroup' -Location 'Nort Europe'

- **VaultName** will be the **KeyVault:Url** that you will use later in the debugSettings.json file.
- ResourceGroupName is the Resource Group Name in Azure.
- Location parameter, use the command [Get-AzureLocation](https://msdn.microsoft.com/library/azure/dn654582.aspx). If you need more information, type: `Get-Help Get-AzureLocation`

Applications that use a key vault must authenticate and has permission granted.

1. Sign in to the Azure Management Portal.
2. On the left, click **Active Directory**, and then select the directory you have used previously.
3. On the Quick Start page, click **CONFIGURE**.
4. Scroll to the **keys** section, select the duration, and then click **SAVE**. The page refreshes and now shows a key value. This value will be **KeyVault:AuthClientSecret** in the config.
5. Copy the client ID value from this page. This value will be  **KeyVault:AuthClientId** in the config.



### Step 5:  Enable the OAuth2 implicit grant for your application

By default, applications provisioned in Azure AD are not enabled to use the OAuth2 implicit grant. In order to run this sample, you need to explicitly opt in.

1. From the former steps, your browser should still be on the Azure management portal - and specifically, displaying the Configure tab of your application's entry.
2. Using the Manage Manifest button in the drawer, download the manifest file for the application and save it to disk.
3. Open the manifest file with a text editor. Search for the `oauth2AllowImplicitFlow` property. You will find that it is set to `false`; change it to `true` and save the file.
4. Using the Manage Manifest button, upload the updated manifest file. Save the configuration of the app.

### Step 6:  Configure the Scamp Application to use your Azure Active Directory tenant

1. TODO:
2. Open the solution in Visual Studio 2015 CTP 6
3. In the Projects -> ScampAPI -> Properties folder, create a file: **debugSettings.json**:



```javascript
{
    "profiles": [
        {
            "name": "IIS Express",
            "launchBrowser": true,
            "launchUrl": "https://localhost:44300/",
            "environmentVariables": {
                "APPSETTING_ClientId": "<clientId-from above App in AAD>",
                "APPSETTING_TenantId": "<tenantId-from above App in AAD->",
                "APPSETTING_RedirectUri": "https://localhost:44300/",
                "APPSETTING_CacheLocation": "localStorage",
                "APPSETTING_DocDb:endpoint": "< URL from https://portal.azure.com >",
                "APPSETTING_DocDb:databaseName": "scamp",
                "APPSETTING_DocDb:collectionName": "scampdata",
                "APPSETTING_DocDb:connectionMode" : "http|tcp",
                "APPSETTING_Provisioning:StorageConnectionString": "<storage connection string>",
								"APPSETTING_KeyVault:Url": "https://{name}.vault.azure.net/",
        				"APPSETTING_KeyVault:AuthClientId": "{Active Directory Client ID}",
        				"APPSETTING_KeyVault:AuthClientSecret": "{Active directory secret}"
            }
        }
    ]
}

```

### Step 6: Configure IIS Express bindings for HTTPS
1. Start the Project - this will launch the site and automatically setup the IIS Express site.
2. It will fail as the bindings for the site to support HTTPS on port 44300 haven't been added yet. We just ran it now to force the creation of the site.
2. Until the VS 2015 CTP tooling has the settings, you have to modify IIS Express configuration.
2. Open your **%USERPROFILE%\Documents\IISExpress\config\applicationhost.config** file
3. locate the <site... element that contains your site
4. Add a new binding as follows so the <../sites/bindings> section looks like:

````
<bindings>
    <binding protocol="http" bindingInformation="*:10838:localhost" />
    <binding protocol="https" bindingInformation="*:44300:localhost" />
</bindings>
````

### Running with specific Tenant and Client ID ###

This makes use of environment variables that need to be added.

In your Package Manager Console, before you debug - add $env variabiels.

    PM> $env:APPSETTING_TenantId = "foo"
    PM> $env:APPSETTING_ClientId = "bar"
    PM> $env:APPSETTING_CacheLocation": "localStorage"
    PM> $env:APPSETTING_DocDb:endpoint = "<url here>"
    PM> $env:APPSETTING_DocDb:authkey = "<key here>"
    PM> $env:APPSETTING_DocDb:databaseName = "<db name here, e.g. scamp>"
    PM> $env:APPSETTING_DocDb:collectionName = "<collection name>"
    PM> $env:APPSETTING_DocDb:connectionMode = "http|tcp"
    PM> $env:APPSETTING_Provisioning:StorageConnectionString = "<azure storage account connection string>"
		PM> $env:APPSETTING_KeyVault:Url = "https://{name}.vault.azure.net/"
		PM> $env:APPSETTING_KeyVault:AuthClientId = "{Active Directory Client ID}"
		PM> $env:APPSETTING_KeyVault:AuthClientSecret = "{Active directory secret}"

Or, these can be set also from Project Properties -> Debug -> Environment Variables to set.
This format is used as this is what AZW uses for Environment variables.


````
APPSETTING_TenantId
APPSETTING_ClientId
APPSETTING_CacheLocation
APPSETTING_DocDb:endpoint
APPSETTING_DocDb:authkey
APPSETTING_DocDb:databaseName
APPSETTING_DocDb:collectionName
APPSETTING_DocDb:connectionMode
APPSETTING_Provisioning:StorageConnectionString
APPSETTING_KeyVault:Url
APPSETTING_KeyVault:AuthClientId
APPSETTING_KeyVault:AuthClientSecret
````

### Settings For Site ###
- **TenantId** this is the Tenant ID of the AAD Domain. This can be retrieved from the Azure Portal from the URL.
- **ClientId** this is the Client ID for the Scamp application once it's been setup in an AAD tenant. This comes from the Applications Configure page for that specific AAD Tenant.
- **CacheLocation** this is a setting that ADAL uses on where 'session' will be managed.
- **DocDB:endpoint** this is the DocumentDB URL that comes from the [Azure Preview Portal](https://portal.azure.com)
- **DocDB:authkey** this is the DocumentDB key that comes from the [Azure Preview Portal](https://portal.azure.com)
- **DocDb:databaseName** this is '**scamp**' by default the Scamp code will create this if it doesn't exist already
- **DocDb:collectionName** this is '**scampdata**' by default the Scamp code will create this if it doesn't exist already.
- **DocDb:connectionMode** specify http for HTTP Mode, or tcp for direct connection to DocumentDB. tcp is recommended if firewalls/proxies allow it. http is likely simplest for local dev
- **Provisioning:StorageConnectionString** this is an Azure Storage Account connection string in the format of:
- **KeyVault:Url** this is the full url of the KeyVault repository (eg.https://scampkeyvault.vault.azure.net/)
- **KeyVault:AuthClientId** this is the client id of the Azure AD that is accessing keyvault
- **KeyVault:AuthClientSecret** this is the secret of the Azure AD app that is accessing keyvault
```
"DefaultEndpointsProtocol=https;AccountName=[AccountName];AccountKey=[AccountKey]"
```

#### Sample Data Generation ####

There is a temporary endpoint to aid the demo scenario (that we should plan to remove). Hit /sampledata to generate sample data. There's still some work to do to generate more meaningful sample data.
