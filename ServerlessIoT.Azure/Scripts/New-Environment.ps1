param (
    [Parameter(Mandatory = $true, HelpMessage = "Environment name")]
    [string] $EnvironmentName,
    [Parameter(Mandatory = $true, HelpMessage = "Location")]
    [string] $Location
)

Import-Module Az.Resources
Import-Module Az.Functions
Import-Module Az.IotHub
Import-Module Az.Storage

Write-Host "Starting creation of Environment '"$EnvironmentName"'"

$randomNamePart = (Get-Random -Minimum 100000 -Maximum 999999 | % {[string]$_})

$resourceGroup= $EnvironmentName + "-rg"
Write-Host "Creating resource group '"$resourceGroup"'"
New-AzResourceGroup -Name $resourceGroup -LOcation $Location

$iotHubName= $EnvironmentName + $randomNamePart +"Hub"
Write-Host "Creating IoTHub '"$iotHubName"'"
New-AzIotHub -Name $iotHubName -ResourceGroupName $resourceGroup -Location $Location -SkuName F1 -Units 1

$storageName = $EnvironmentName.ToLower() + $randomNamePart 
Write-Host "Creating storage account '"$storageName"'"
New-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storageName -Location $Location -SkuName Standard_LRS

$functionAppName = $EnvironmentName + "FuncApp"
Write-Host "Creating function app '"$functionAppName"'"
New-AzFunctionApp -ResourceGroupName $resourceGroup -Name $functionAppName -Location $Location -Runtime DotNet -FunctionsVersion 3 -StorageAccountName $storageName -OSType Windows
