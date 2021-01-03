# Docker Commands

## Create docker image

To create docker image:

1. Open cmd 
2. Goto root folder for the solution
3. Build image using the command

`docker build -f .\Frontend\TelemetrySimulator\dockerfile -t telemetrysimulator D:\Sviluppo\ServerlessIoT`


## Run docker image

To run docker image:

1. Run the command

`docker run -a STDOUT <imageId> -f <configuration file path>`

You can use the following options:

- `-f <configuration file path>` where `<configuration file path>` is the full path of the JSON file contains the devices configuration;
- `-b <configuration blob url>` where `<configuration blob url>` is the url of the blob (JSON configuration file) contains the devices configuration;

To retrieve the `<imageId>` run the command:

`docker image ls telemetrysimulator`