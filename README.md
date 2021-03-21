# Orca
Repository for the UCL ORCA project


# Installation
## Sharepoint and MS Teams Permissions
Since ORCA stores attendance information and reports on Sharepoint sites and records attendance from MS Teams, it needs permission to access the relevant resources. To do so, an application must be registered with [Sharepoint credentials](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs) which allow it to modify the relevant Sharepoint sites.


### Register a Sharepoint Application for ORCA
Navigate to a site in your tenant (e.g. https://contoso.sharepoint.com) and then call the appregnew.aspx page (e.g. https://contoso.sharepoint.com/_layouts/15/appregnew.aspx). In this page click on the Generate button to generate a `Client Id` and `Client Secret`. Copy the Client Id and Client Secret and keep track of them as you will use them later. Fill the remaining fields as below:  
Title: ORCA  
App Domain: www.localhost.com  
Redirect URI: https://www.localhost.com  
**⚠️ Make sure to keep track of your Sharepoint Client Id and Client Secret since you'll need them later when configuring ORCA**


### Give permissions to the Sharepoint Application
After registering the Sharepoint Application, go to the appinv.aspx page on the tenant administration site. You can reach this site via https://contoso-admin.sharepoint.com/_layouts/15/appinv.aspx (replace contoso with your sharepoint tenant name). Once the page is loaded add your client id and look up the created principal.
In the Permission Requests XML section, add the following:
```xml
<AppPermissionRequests AllowAppOnlyPolicy="true">
      <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl"/>
</AppPermissionRequests>
```
Then select "Create" and choose to trust the application.


### Register an Azure Application with MS Teams access for ORCA
TODO


## Deployment and Configuration
ORCA can be automatically deployed to the Azure cloud or configured to run on a custom (on-premises) windows, linux, or mac-os machine of your choosing. Powershell scripts are provided to automate configuration and deployment, although you can manually change ORCA's centralized configuration. 


### Deploying ORCA to Azure
Download the zipped ORCA binaries for your OS from the [Releases](https://github.com/Beyhum/Orca/releases) page. Once unzipped, under the `orca-{os_name}/Scripts` directory (e.g.orca-win-x64/Scripts) you will find a Powershell script named `DeployToAzureAndConfigureOrca.ps1`.  
The script will connect you to your Azure account and automatically provision/configure the required resources to run Orca. Make sure to have the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) installed before running the script.
Once run the script will ask you to fill in the required settings to deploy ORCA, such as the [Sharepoint Client Id and Client Secret that you generated in earlier steps](#Register-a-Sharepoint-Application-for-ORCA).  
**⚠️ The script sets sensible defaults for the resources to deploy such as the size and price of the web server running ORCA and the optional database. You can change these advanced settings in the [ARM template](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/overview) used under `orca-{os_name}/Scripts/AzureResourceManager/template.json`'s `parameters` section.**


### Deploying ORCA to a server of your choosing
Download the zipped ORCA binaries for your OS from the [Releases](https://github.com/Beyhum/Orca/releases) page. Once unzipped, under the `orca-{os_name}/Scripts` directory (e.g.orca-win-x64/Scripts) you will find a Powershell script named `ConfigureOrcaAppSettings.ps1`.  
Once run the script will ask you to fill in the required settings to configure the `orca-{os_name}/appsettings.json` file, such as the [Sharepoint Client Id and Client Secret that you generated in earlier steps](#Register-a-Sharepoint-Application-for-ORCA).  
After configuring the application, you can run it through the executable under `orca-{os_name}/Orca.exe`. For a more robust setup consider running ORCA behind a reverse proxy (e.g. [Nginx](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0#configure-nginx)).