# TelemetryDispatcher command line arguments

To run Telemetry Dispatcher use the following command line:

```bash
.\TelemetryDispatcher.exe <options>
```
where:
- `-e` (`--EventHubCompatibleEndpoint`) The event hub-compatible endpoint from your IoT Hub instance. Use `az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}` to fetch via the Azure CLI.;
- `-n` (`--EventHubName`) The event hub-compatible name of your IoT Hub instance. Use `az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}` to fetch via the Azure CLI.
- `-a` (`--SharedAccessKey`) A primary or shared access key from your IoT Hub instance, with the 'service' permission. Use `az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}` to fetch via the Azure CLI.
- `-c` (`--EventHubConnectionString`) The connection string to the event hub-compatible endpoint. Use the Azure portal to get this parameter. If this value is provided, `-a` `-n` `-e` are not necessary.
- `-s` (`--StorageConnectionString`) The storage connectionstring for storage account uses by the dispatcher to store the offset for messages retrieved from the iot hub.
- `-b` (`--BlobContainerName`) The name of blob container uses by the dispatcher to store the offset for messages retrieved from the iot hub (more info [here](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send)). 
- `-g` (`--ConsumerGroup`) The consumer group name uses by the dispatcher connect to the iot hub. The default value is `$Default`.
- `-u` (`--EntitiesBaseAPIUrl`) The base URL for the Device Entities APIs exposed by the Function App that contains the device entities.
- `-k` (`--EntitiesAPIKey`) The API Key for the Device Entities APIs exposed by the Function App that contains the device entities.