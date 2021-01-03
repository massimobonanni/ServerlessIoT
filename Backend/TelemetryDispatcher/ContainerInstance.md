# Container Instance

## Deploy container instance

To deploly a container instance:

1. Open Cloud Shell (local or using Azure portal) 
2. Run the following command

`az container create --resource-group <resource group name> --name <container instance name> --image <ACR Name>.azurecr.io/<image name> --command-line "dotnet TelemetryDispatcher.dll -c '<IoT event hub-compatible connection string>'" --registry-username <ACR username> --registry-password <ACR password>`

where:

- `<resource group name>` is the name of the resource group which you want to create the instance; 
- `<container instance name>` is the name of the container instance (e.g. `telemetrydispatcher`);
- `<ACR Name>` is the name of the Azure Container Registry that contains the image of the TelemetryDispatcher;
- `<image name>` is the name of the container image in the ACR;
- `<IoT event hub-compatible connection string>` is the connection string for Event-Hub compatible endpoint of the IoT Hub;
- `<ACR username>` is the ACR username (you can find it in the portal);
- `<ACR password>` is the ACR passwortd (you can find it in the portal).



