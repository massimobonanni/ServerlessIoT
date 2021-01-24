param (
    [Parameter(Mandatory = $true, HelpMessage = "Resource group name")]
    [string] $ResourceGroup,
    [Parameter(Mandatory = $true, HelpMessage = "IotHub name")]
    [string] $IotHubName,
    [Parameter(Mandatory = $true, HelpMessage = "Number of devices to create")]
    [int] $DeviceNumber,
    [Parameter(Mandatory = $true, HelpMessage = "Full path of the JSON file to create for simulator")]
    [string]  $JSONFileName,
    [Parameter(Mandatory = $true, HelpMessage = "Prefix name for the devices. The nema of each device will be generated randomly using this parameter as prefix")]
    [string]  $PrefixDeviceName,
    [Parameter( HelpMessage = "Polling interval in seconds used by the simulator to send telemetries. Default 5 secs.")]
    [int] $PollingIntervalInSec = 5
)

Import-Module az.iothub

Write-Host "Starting creation of " $DeviceNumber "device for iotHub '"$IotHubName"'"

$devices = New-Object System.Collections.ArrayList

For ($i=0; $i -le $DeviceNumber; $i++) {
    
    $deviceName = $PrefixDeviceName + $i;

    Write-Host "Creating device '"$deviceName"'"

    Add-AzIotHubDevice -ResourceGroupName $ResourceGroup -IotHubName $IotHubName -DeviceId $deviceName -AuthMethod shared_private_key

    Write-Host "Retrieving ConnectionString for device '"$deviceName"'"

    $connection = Get-AzIotHubDeviceConnectionString -ResourceGroupName $ResourceGroup -IotHubName $IotHubName -DeviceId $deviceName

    Write-Host "ConnectionString : "$connection.ConnectionString

    $device = @{'connectionString' = $connection.ConnectionString; 'name' = $deviceName ; 'id' = $deviceName ; 'pollingIntervalInSec' = $PollingIntervalInSec}

    $devices.Add($device)
}

$config= @{'devices' = $devices}

$configJson=ConvertTo-Json -InputObject $config

Write-Host "Writing file '"$JSONFileName"'"

Out-File -FilePath $JSONFileName -InputObject $configJson







