# Deploy-TelemetryDispatcherACI cmdlet

This script allow you to create an instance of Azure Container Instance base on the TelemetryDispatcher image.

```powershell
Deploy-TelemetryDispatcherACI.ps1 -ResourceGroup <resource group name> 
                                  -IotHubName <IoTHub name> 
                                  -EventHubConnectionString <event hub compatible connectionstring> 
                                  -StorageAccountName <storage account name> 
                                  -FunctionAppName <functionapp name>
```

where:
- `-ResourceGroup` the name of the resource group 
- `-IotHubName` the IotHub name 
- `-EventHubConnectionString` EventHub compatible connectionstring for IoTHub 
- `-StorageAccountName` StorageAccount name used by the dispatcher to save offset for iothub events 
- `-FunctionAppName` Function App name contains the Durable Entities




