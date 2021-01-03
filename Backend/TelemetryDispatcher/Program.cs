// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/eventhub/Azure.Messaging.EventHubs/samples/README.md
// For documentation see: https://docs.microsoft.com/azure/event-hubs/

using Azure.Messaging.EventHubs.Consumer;
using CommandLine;
using Newtonsoft.Json;
using ServerlessIoT.Core;
using ServerlessIoT.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryEntities.Rest;

namespace TelemetryDispatcher
{
    /// <summary>
    /// A sample to illustrate reading Device-to-Cloud messages from a service app.
    /// </summary>
    internal class Program
    {
        private static Parameters _parameters;
        private static ITelemetryManager _telemetryManager;

        public static async Task Main(string[] args)
        {
            // Parse application parameters
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(parsedParams =>
                {
                    _parameters = parsedParams;
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });

            // Either the connection string must be supplied, or the set of endpoint, name, and shared access key must be.
            if (string.IsNullOrWhiteSpace(_parameters.EventHubConnectionString)
                && (string.IsNullOrWhiteSpace(_parameters.EventHubCompatibleEndpoint)
                    || string.IsNullOrWhiteSpace(_parameters.EventHubName)
                    || string.IsNullOrWhiteSpace(_parameters.SharedAccessKey)))
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(result, null, null));
                Environment.Exit(1);
            }

            _telemetryManager = new DeviceEntityManagementRestProvider(new HttpClient(),
                _parameters.EntitiesAPIUrl, _parameters.EntitiesAPIKey);

            Console.WriteLine("Read device to cloud messages. Ctrl-C to exit.\n");

            // Set up a way for the user to gracefully shutdown
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            // Run the sample
            await ReceiveMessagesFromDeviceAsync(cts.Token);

            Console.WriteLine("Cloud message reader finished.");
        }

        // Asynchronously create a PartitionReceiver for a partition and then start
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(CancellationToken ct)
        {
            string connectionString = _parameters.GetEventHubConnectionString();

            await using var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                connectionString,
                _parameters.EventHubName);

            Console.WriteLine("Listening for messages on all partitions.");

            try
            {
                await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync(ct))
                {
                    LogPartitionEvent(partitionEvent);
                    if (_parameters.IsAPIEndpointEnabled())
                        await SendPartitionEventToEntitiesAsync(partitionEvent, ct);
                }
            }
            catch (TaskCanceledException)
            {

            }
        }

        private static async Task SendPartitionEventToEntitiesAsync(PartitionEvent partitionEvent, CancellationToken ct)
        {
            var deviceTelemetry = partitionEvent.ToDeviceTelemetry();
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

        private static void LogPartitionEvent(PartitionEvent partitionEvent)
        {
            Console.WriteLine($"\nMessage received on partition {partitionEvent.Partition.PartitionId}:");

            string data = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
            Console.WriteLine($"\tMessage body: {data}");

            Console.WriteLine("\tApplication properties (set by device):");
            foreach (KeyValuePair<string, object> prop in partitionEvent.Data.Properties)
            {
                Console.WriteLine($"\t\t{prop.Key}: {prop.Value}");
            }

            Console.WriteLine("\tSystem properties (set by IoT Hub):");
            foreach (KeyValuePair<string, object> prop in partitionEvent.Data.SystemProperties)
            {
                Console.WriteLine($"\t\t{prop.Key}: {prop.Value}");
            }
        }
    }
}
