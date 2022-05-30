param environmentName string
param location string
param eventHubCompatibleEndpoint string
param eventHubCompatibleName string

param keyVaultName string = '${replace(environmentName, ' ', '-')}-keyvault'

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: []
  }
}

resource EventHubCompatibleConnectionString 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${keyVault.name}/EventHubCompatibleConnectionString'
  properties: {
    value: eventHubCompatibleEndpoint
  }
}

resource IotHubNameAppSettings 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${keyVault.name}/IotHubName'
  properties: {
    value: eventHubCompatibleName
  }
}

output keyVaultName string =  keyVault.name
output ioTHubConnectionSettingKVSecret string = EventHubCompatibleConnectionString.properties.secretUri
output ioTHubNamenSettingKVSecret string = IotHubNameAppSettings.properties.secretUri
