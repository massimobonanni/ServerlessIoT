param (
    [Parameter(Mandatory = $true, HelpMessage = "Resource group name")]
    [string] $ResourceGroup,
    [Parameter(Mandatory = $true, HelpMessage = "ACI location")]
    [string] $Location,
    [Parameter(Mandatory = $true, HelpMessage = "URL for the blob contains JSON configuration for the simulator")]
    [string] $ConfigurationBlobUrl,
    [Parameter(HelpMessage = "Duration (in seconds) of the simulation. If you use 0, the simulation never ends. Default value 0.")]
    [int] $Duration = 0
)

Import-Module Az.Resources

$dateNamePart = (Get-Date -Format "yyyyMMddHHmmss" | % {[string]$_})

$aciName="telemetrysimulator" + $dateNamePart
$containerImage="massimobonanni/telemetrysimulator:v2.0"

Write-Host "Deploying ACI '"$aciName"'"
$containerCommand="dotnet TelemetrySimulator.dll -b '"+ $ConfigurationBlobUrl +"'"
if($Duration -gt 0){
    $containerCommand=$containerCommand + " -d "+ $Duration 
} 

New-AzContainerGroup -ResourceGroupName $ResourceGroup -Name $aciName -Image $containerImage -Location $Location -OsType Linux -Command $containerCommand -Cpu 2 -MemoryInGB 4 -RestartPolicy Never

Get-AzContainerGroup -ResourceGroupName $ResourceGroup -name $aciName
