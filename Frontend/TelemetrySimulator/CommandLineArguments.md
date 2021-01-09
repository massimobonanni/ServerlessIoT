# TelemetrySimulator command line arguments

To run Telemetry Simulator use the following command line:

```bash
.\TelemetrySimulator.exe <options>
```
where:
- `-f` (`--ConfigFile`) Json File contains the devices configuration;
- `-b` (`--BlobUrl`) TUrl for the blob contains JSON configuration for the devices.
- `-d` (`--Duration`)Duration of the simulation (in seconds). If you don't set this options, the simuilator runs until you stop it.