# Serverless IoT - Azure

This project contains all the templates and scripts you need to create and manage the Azure resources you need to deploy a full environment

## Bicep Template
Deploy Bicep template with the following command:

```bash
az deployment sub create --location <location> --template-file .\ServerlessIoTEnvironment.bicep --parameters environmentName=<name of your environment>
```

where:
* `location` : Azure region where you want to deploy the solution;
* `environment name` : the name of your environment.



## Powershell Scripts

- Generate a set of devices in IoTHub and the simulator JSON file ([here](Scripts/Add-SimulatorDevices.md))
- Generate full environment for the demo ([here](Scripts/New-Environment.md))
- Create a ACI with TelemetryDispatcher image ([here](Scripts/Deploy-TelemetryDispatcherACI.md))
- - Create a ACI with TelemetrySimulator image ([here](Scripts/Deploy-TelemetrySimulatorACI.md))
