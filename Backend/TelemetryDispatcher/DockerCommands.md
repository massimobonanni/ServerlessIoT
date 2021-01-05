# Docker Commands

## Create docker image

To create docker image:

1. Open cmd 
2. Goto root folder for the solution
3. Build image using the command

`docker build -f .\Backend\TelemetryDispatcher\dockerfile -t telemetrydispatcher D:\Sviluppo\ServerlessIoT`


## Run docker image

To run docker image:

1. Run the command

`docker run -a STDOUT <imageId> <command line argument>`

More info about `<command line options>` on the page [Command Line Arguments](CommandLineArguments.md).

To retrieve the `<imageId>` run the command:

`docker image ls telemetrydispatcher`