param environmentName string
param location string

param ioTHubName string = '${replace(environmentName, ' ', '-')}-hub'

resource IotHub 'Microsoft.Devices/IotHubs@2021-07-02' = {
  name: ioTHubName
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
  identity: {
    type: 'None'
  }
  properties: {
    ipFilterRules: []
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: 2
      }
    }
    routing: {
      endpoints: {
        serviceBusQueues: []
        serviceBusTopics: []
        eventHubs: []
        storageContainers: []
      }
      routes: []
      fallbackRoute: {
        name: '$fallback'
        source: 'DeviceMessages'
        condition: 'true'
        endpointNames: [
          'events'
        ]
        isEnabled: true
      }
    }
    enableFileUploadNotifications: false
    features: 'None'
  }
}

output IotHubName string = IotHub.name
output eventHubCompatibleIoTHubEndpoint string = 'Endpoint=${IotHub.properties.eventHubEndpoints['events'].endpoint};SharedAccessKeyName=iothubowner;SharedAccessKey=${listKeys(IotHub.id, providers('Microsoft.Devices', 'IoTHubs').apiVersions[0]).value[0].primaryKey}'
output eventHubCompatibleIoTHubName string = IotHub.properties.eventHubEndpoints['events'].path
