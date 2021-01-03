using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using CommandLine;
using Newtonsoft.Json;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Rest;

namespace TelemetryDispatcher
{
    internal class Program
    {
        private static Parameters _parameters;
        private static ITelemetryManager _telemetryManager;

        public static async Task Main(string[] args)
        {
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(parsedParams =>
                {
                    _parameters = parsedParams;
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });

            if (!_parameters.IsValid())
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(result, null, null));
                Environment.Exit(1);
            }

            _telemetryManager = new DeviceEntityManagementRestProvider(new HttpClient(),
                _parameters.EntitiesAPIUrl, _parameters.EntitiesAPIKey);

            Console.WriteLine("Read device to cloud messages. Ctrl-C to exit.\n");

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            BlobContainerClient storageClient =
                new BlobContainerClient(_parameters.StorageConnectionString, _parameters.BlobContainerName);

            EventProcessorClient processor = new EventProcessorClient(
                storageClient,
                _parameters.ConsumerGroupName,
                _parameters.GetEventHubConnectionString(),
                _parameters.EventHubName,
               new EventProcessorClientOptions()
               {
                   Identifier="TelemetryDispatcher"
               }
            );

            processor.ProcessEventAsync += processEventHandler;
            processor.ProcessErrorAsync += processErrorHandler;

            await processor.StartProcessingAsync();

            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                await processor.StopProcessingAsync();
            }
            finally
            {
                // To prevent leaks, the handlers should be removed when processing is complete
                processor.ProcessEventAsync -= processEventHandler;
                processor.ProcessErrorAsync -= processErrorHandler;
            }

            Console.WriteLine("Cloud message reader finished.");
        }

        private static async Task processEventHandler(ProcessEventArgs eventArgs)
        {
            try
            {
                LogProcessEventArgs(eventArgs);
                if (_parameters.IsAPIEndpointEnabled())
                    await SendProcessEventArgsToEntitiesAsync(eventArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] processEventHandler -> {ex.Message}");
            }
        }

        private static Task processErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            try
            {
                Console.WriteLine($"\n[ERROR] processErrorHandler -> Operation '{eventArgs.Operation}' from Partition {eventArgs.PartitionId} - {eventArgs.Exception.Message} ");
            }
            catch
            {

            }
            return Task.CompletedTask;
        }

        private static async Task SendProcessEventArgsToEntitiesAsync(ProcessEventArgs eventArgs, CancellationToken ct = default)
        {
            var deviceTelemetry = eventArgs.ToDeviceTelemetry();
            if (deviceTelemetry != null)
            {
                try
                {
                    await _telemetryManager.SendTelemetryToDeviceAsync(deviceTelemetry, ct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EXCEPTION] - Exception during sending telemetry to entities - {ex.Message}");
                }
            }
        }

        private static void LogProcessEventArgs(ProcessEventArgs eventArgs)
        {
            Console.WriteLine($"\nMessage received on partition {eventArgs.Partition.PartitionId}:");

            string data = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            Console.WriteLine($"\tMessage body: {data}");

            Console.WriteLine("\tApplication properties (set by device):");
            foreach (KeyValuePair<string, object> prop in eventArgs.Data.Properties)
            {
                Console.WriteLine($"\t\t{prop.Key}: {prop.Value}");
            }

            Console.WriteLine("\tSystem properties (set by IoT Hub):");
            foreach (KeyValuePair<string, object> prop in eventArgs.Data.SystemProperties)
            {
                Console.WriteLine($"\t\t{prop.Key}: {prop.Value}");
            }
        }

    }
}
