# New-Environment cmdlet

This script allow you to create a full environment for the demo composed by Resource Group, IotHub, Storage Account and Function App.

```powershell
New-Environment.ps1 -EnvironmentName <environment name> 
                    -Location <azure location>
```

where:
- `-EnvironmentName` name of the environment. The cmdlet use this paremeter to create the names for resource group, IotHub, storage account e function app. 
- `-Location` location for the azure resources 




