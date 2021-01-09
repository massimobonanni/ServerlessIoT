# Deploy-TelemetrySimulatorACI cmdlet

This script allow you to create an instance of Azure Container Instance base on the TelemetrySimulator image.

```powershell
Deploy-TelemetrySim,ulatorACI.ps1 -ResourceGroup <resource group name> 
                                  -Location <Location> 
                                  -ConfigurationBlobUrl <config blob url>
                                  -Duration <seconds>
```

where:
- `-ResourceGroup` the name of the resource group 
- `-Location` location for the ACI 
- `-ConfigurationBlobUrl` URL for the blob contains JSON configuration for the simulator
- `-Duration` Duration (in seconds) of the simulation. If you use 0, the simulation never ends. Default value 0.



