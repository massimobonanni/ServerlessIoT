# Add-SimulatorDevices cmdlet

This script allow you to create a set of devices (with the same prexi in the name), retrieve the connection string for each of those and create the simulator JSON file for test.

```powershell
Add-SimulatorDevices.ps1 -ResourceGroup <resource group name> 
                         -IotHubName <IoTHub name> 
                         -DeviceNumber <number of device to generate> 
                         -JSONFileName <full JSON filename> 
                         -PrefixDeviceName <Prefix device name> 
                         -PollingIntervalInSec <polling interval>
```

where:
- `-ResourceGroup` the name of the resource group 
- `-IotHubName` the IotHub name 
- `-DeviceNumber` number of devices to generate 
- `-JSONFileName` full JSON filename to create 
- `-PrefixDeviceName` all the devices will be created with a name that will start with this prefix 
- `-PollingIntervalInSec` the time interval used by the divices to send telemetry during the simulation




