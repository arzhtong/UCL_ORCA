Write-Host "Configuring appsettings.json"

# Read appsettings.json
function Get-ScriptDirectory { Split-Path $MyInvocation.ScriptName }
$appsettingsPath = Join-Path (Get-ScriptDirectory) 'appsettings.Template.json'
$appsettings = Get-Content $appsettingsPath -Raw

#Replace values for sharepoint
Write-Host "Please provide the following settings to configure the application:"

$sharepointUrl = Read-Host "Your Sharepoint site url (e.g. https://myinstitution.sharepoint.com/myCustomSite)"
$appsettings = $appsettings -replace "//`"SharepointUrl`": `"`"", "`"SharepointUrl`": `"$sharepointUrl`""

$clientId = Read-Host "Your Sharepoint Client Id "
$appsettings = $appsettings -replace "//`"ClientId`": `"`"", "`"ClientId`": `"$clientId`""

$sharepointClientSecret = Read-Host "Your Sharepoint Client Secret "
$appsettings = $appsettings -replace "//`"ClientSecret`": `"YOUR_SHAREPOINT_CLIENT_SECRET`"", "`"ClientSecret`": `"$sharepointClientSecret`""

#Replace values for msgraph
$appId = Read-Host "Your Azure App Id with Microsoft Graph Access"
$appsettings = $appsettings -replace "//`"AppId`": `"`"", "`"AppId`": `"$appId`""

$tenantId = Read-Host "Your Azure Tenant Id"
$appsettings = $appsettings -replace "//`"TenantId`": `"`"", "`"TenantId`": `"$tenantId`""

$msGraphClientSecret = Read-Host "Your Azure ClientSecret with Microsoft Graph Access"
$appsettings = $appsettings -replace "//`"ClientSecret`": `"YOUR_MS_GRAPH_CLIENT_SECRET`"", "`"ClientSecret`": `"$msGraphClientSecret`""

$domain = Read-Host "Your public host URL through which this application is exposed (e.g. https://myorcadeployment.azurewebsites.net)"
$appsettings = $appsettings -replace "//`"Domain`": `"`"", "`"Domain`": `"$domain`""

#Replace values for database
$enableDatabase = Read-Host "Would you like to connect ORCA to a MySQL database to enable analytics?`n([Y]es/[N])o"
If ($enableDatabase.ToUpper() -match "Y" -or $enableDatabase.ToUpper() -match "YES") {
    $serverName = Read-Host "Database Server Name"
    $appsettings = $appsettings -replace "//`"Servername`": `"`"", "`"Servername`": `"$serverName`""
	
    $Uid = Read-Host "Database User ID"
    $appsettings = $appsettings -replace "//`"Uid`": `"`"", "`"Uid`": `"$Uid`""

    $Password = Read-Host "Database Password"
    $appsettings = $appsettings -replace "//`"Password`": `"`"", "`"Password`": `"$Password`""	
	
	$Database = Read-Host "Database name"
    $appsettings = $appsettings -replace "//`"Database`": `"`"", "`"Database`": `"$Database`""	
	
}

# Write to appsettings.json
$configuredAppsettingsPath = Join-Path (Get-ScriptDirectory) '../appsettings.json'
Set-Content -Path $configuredAppsettingsPath -Value $appsettings
Write-Host "Configuration saved to appsettings.json"


