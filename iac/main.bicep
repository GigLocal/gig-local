// Name of this app
param appName string

// Environment name for deployment
param env string

// Storage account container name
param containerName string

// Web App params
@allowed([
    'B1'
])
param webAppSku string

@minValue(1)
param webAppSkuCapacity int

// Data params
param sqlAdminLogin string

@secure()
param sqlAdminPassword string

param sqlDatabaseName string

@allowed([
    'Basic'
])
param sqlDatabaseSku string

// Auth params
param authGoogleClientId string
param authGoogleClientSecret string
param authAdminEmail string

var location = resourceGroup().location
var resourceNameSuffix = '${appName}${env}${uniqueString(resourceGroup().id)}'

// Variables
var hostingPlanName = 'HostingPlan${resourceNameSuffix}'
var websiteName = 'Website${resourceNameSuffix}'
var sqlServerName = 'SqlServer${resourceNameSuffix}'
var storageName = toLower(resourceNameSuffix)
var appInsightsName = 'AppInsights${resourceNameSuffix}'

// Storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
    name: storageName
    location: location
    sku: {
        name: 'Standard_LRS'
    }
    kind: 'StorageV2'
}
resource storageAccountContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
    name: '${storageAccount.name}/default/${containerName}'
    properties: {
        publicAccess: 'Blob'
    }
}

// Data resources
resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
  }
}
resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  name: '${sqlServer.name}/${sqlDatabaseName}'
  location: location
  sku: {
    name: sqlDatabaseSku
  }
}
resource sqlServerFirewall 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  name: '${sqlServer.name}/AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

// Web App resources
resource hostingPlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: hostingPlanName
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: webAppSku
    capacity: webAppSkuCapacity
  }
}
resource website 'Microsoft.Web/sites@2021-01-15' = {
  name: websiteName
  location: location
  tags: {
    'hidden-related:${hostingPlan.id}': 'empty'
    displayName: 'Website'
  }
  properties: {
    serverFarmId: hostingPlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|6.0'
      http20Enabled: true
    }
  }
}

// Monitor
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
    name: appInsightsName
    location: location
    tags: {
      'hidden-link:${website.id}': 'Resource'
      displayName: 'AppInsightsComponent'
    }
    kind: 'web'
    properties: {
      Application_Type: 'web'
    }
  }

// Configuration
var sqlConnectionString = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

resource websiteConnectionStrings 'Microsoft.Web/sites/config@2021-01-15' = {
  name: '${website.name}/connectionstrings'
  properties: {
    DefaultConnection: {
      value: sqlConnectionString
      type: 'SQLAzure'
    }
  }
}
resource websiteAppSettings 'Microsoft.Web/sites/config@2021-01-15' = {
  name: '${website.name}/appsettings'
  properties: {
    'Storage__ConnectionString': storageConnectionString
    'Authentication__Google__ClientId': authGoogleClientId
    'Authentication__Google__ClientSecret': authGoogleClientSecret
    'Authentication__Admin__Email': authAdminEmail
    'ApplicationInsights__ConnectionString': appInsights.properties.ConnectionString
  }
}

output websiteName string = websiteName
output sqlServerName string = sqlServer.properties.fullyQualifiedDomainName
output sqlConnectionString string = sqlConnectionString
