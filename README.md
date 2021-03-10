# Orca
Repository for the UCL ORCA project

To run the subscription

1. download ngrok https://ngrok.com/download
2. run ngrok [./ngrok http https://localhost:5002]
3. replace Ngrok link in appsettings.json to the new https forwarding address 
4. run the app https://github.com/microsoftgraph/msgraph-training-changenotifications/blob/master/tutorial/06_run.md


# Required Setup
## Sharepoint and MS Teams Permissions
Since ORCA stores attendance information and reports on Sharepoint sites and records attendance from MS Teams, it needs permission to access the relevant resources. To do so, an application must be registered with [Sharepoint credentials](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azureacs) which allow it to modify the relevant Sharepoint sites.

### Register a Sharepoint Application for ORCA
Navigate to a site in your tenant (e.g. https://contoso.sharepoint.com) and then call the appregnew.aspx page (e.g. https://contoso.sharepoint.com/_layouts/15/appregnew.aspx). In this page click on the Generate button to generate a `Client Id` and `Client Secret`. Copy the Client Id and Client Secret and keep track of them as you will use them later. Fill the remaining fields as below:  
Title: ORCA  
App Domain: www.localhost.com  
Redirect URI: https://www.localhost.com  

### Give permissions to the Sharepoint Application
After registering the Sharepoint Application, go to the appinv.aspx page on the tenant administration site. You can reach this site via https://contoso-admin.sharepoint.com/_layouts/15/appinv.aspx (replace contoso with your sharepoint tenant name). Once the page is loaded add your client id and look up the created principal.
In the Permission Requests XML section, add the following:
```xml
<AppPermissionRequests AllowAppOnlyPolicy="true">
      <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl"/>
</AppPermissionRequests>
```
Then select "Create" and choose to trust the application.
Finally in your appsettings.json, set Orca:Sharepoint:ClientId and Orca:Sharepoint:ClientSecret to the values generated in the previous step.