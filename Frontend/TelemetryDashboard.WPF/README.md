# TelemetryDashboard.WPF

This project is a sample of a dashboard that interact with the APIs exposed by the Azure Functions project (TelemetryEntities) to show, in real-time, all the devices and, for each device, the telemetries stored in the entities.

## Command arguments

To run Telemetry Dashboard use the following command line:

```bash
.\TelemetryDashboard.WPF.exe <options>
```
where:
- `-u` (`--apiurl`) url of the Azure Function App that host the telemetry APIs;
- `-k` (`--apikey`) key to use to invoke the Azure Functions in the Azure Function App (you can find it in the Azure portal);
- `-p` (`--pollingtime`) polling time (in seconds) between each call to the APIs to retrieve the telemetries for a device.

## WPF Chart
This project uses Syncfusion Chart to show date from devices.
You can use the project even if you don't have a valid Syncfusion license. See [remove Syncfusion controls](RemoveSyncfusionControl.md) to understand how or understand how you can set your own license.