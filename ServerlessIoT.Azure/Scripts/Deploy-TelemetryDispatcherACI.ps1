param (
    [Parameter(Mandatory = $true, HelpMessage = "Resource group name")]
    [string] $ResourceGroup,
    [Parameter(Mandatory = $true, HelpMessage = "IotHub name")]
    [string] $IotHubName,
    [Parameter(Mandatory = $true, HelpMessage = "EventHub compatible connectionstring for IoTHub")]
    [string] $EventHubConnectionString,
    [Parameter(Mandatory = $true, HelpMessage = "StorageAccount name used by the dispatcher to save offset for iothub events")]
    [string] $StorageAccountName,
    [Parameter(Mandatory = $true, HelpMessage = "Function App name contains the Durable Entities")]
    [string] $FunctionAppName
)

# EventHubConnectionString --> iothubname
# BlobName --> creato
# StorageConnectionString --> StorageAccount
# EntitiesBaseAPIUrl --> FunctionAppName
 
Import-Module Az.Resources
Import-Module Az.Functions
Import-Module Az.IotHub
Import-Module Az.Storage

$dateNamePart = (Get-Date -Format "yyyyMMddHHmmss" | % {[string]$_})

$aciName="telemetrydispatcher" + $dateNamePart
$containerImage="massimobonanni/telemetrydispatcher:v2.0"

$storageBlobName="offset"+$dateNamePart

Write-Host "Retrieving IotHub data '"$IotHubName"'"

$iothub= Get-AzIotHub -ResourceGroupName $ResourceGroup -Name $IotHubName

Write-Host "Retrieving Storage Account data '"$StorageAccountName"'"
$storageAccount = Get-AzStorageAccount -ResourceGroupName $ResourceGroup -Name $StorageAccountName

$storageAccountContext= New-AzStorageContext -StorageAccountName $StorageAccountName -UseConnectedAccount
$blobContainer = New-AzStorageContainer -Name $storageBlobName -Context $storageAccountContext -Permission Container

$storageKey=(Get-AzStorageAccountKey -ResourceGroupName $ResourceGroup -Name $StorageAccountName).Value[0]

$storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=" + $StorageAccountName +";AccountKey=" + $storageKey +";EndpointSuffix=core.windows.net"

Write-Host "Retrieving Function App data '"$FunctionAppName"'"
$functionAppUrl= "https://"+ $FunctionAppName +"azurewebsites.net"

Write-Host "Deploying ACI '"$aciName"'"
$containerCommand="dotnet TelemetryDispatcher.dll -c '"+ $EventHubConnectionString +"' -b '"+$storageBlobName+"' -s '"+$storageConnectionString+"' -u '"+$functionAppUrl +"'"
New-AzContainerGroup -ResourceGroupName $ResourceGroup -Name $aciName -Image $containerImage -Location $iothub.Location -OsType Linux -Command $containerCommand -Cpu 4 -MemoryInGB 8

Get-AzContainerGroup -ResourceGroupName $ResourceGroup -name $aciName
