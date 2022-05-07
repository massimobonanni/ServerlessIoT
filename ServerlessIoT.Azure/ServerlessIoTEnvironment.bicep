targetScope = 'subscription'

param environmentName string
param location string = deployment().location

var resourceGroupName = '${replace(environmentName, ' ', '-')}-rg'

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module IoTModule 'IoTModule.bicep' = {
  scope: resourceGroup
  name: 'IoT'
  params: {
    environmentName: environmentName
    location: location
  }
}

module KeyVaultModule 'KeyVaultModule.bicep' = {
  scope: resourceGroup
  name: 'KeyVault'
  params: {
    environmentName: environmentName
    location: location
    eventHubCompatibleEndpoint : IoTModule.outputs.eventHubCompatibleIoTHubEndpoint
    eventHubCompatibleName: IoTModule.outputs.eventHubCompatibleIoTHubName
  }
}

module FunctionsModule 'FunctionsModule.bicep' = {
  scope: resourceGroup
  name: 'Functions'
  params: {
    environmentName: environmentName
    location: location
    keyVaultName: KeyVaultModule.outputs.keyVaultName
    ioTHubConnectionSettingKVSecret:KeyVaultModule.outputs.ioTHubConnectionSettingKVSecret
    ioTHubNamenSettingKVSecret:KeyVaultModule.outputs.ioTHubNamenSettingKVSecret
  }
}
