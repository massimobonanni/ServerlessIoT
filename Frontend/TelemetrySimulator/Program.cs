using CommandLine;
using Microsoft.Azure.Devices.Client;
using ServerlessIoT.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace TelemetrySimulator
{
    class Program
    {
        private static Parameters _parameters;

        static async Task Main(string[] args)
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

            if (_parameters.IsValid())
                Environment.Exit(1);

            Console.WriteLine("Telemetry simulator.");

            DevicesConfiguration config = null;
            if (_parameters.IsFileConfig())
            {
                config = await ReadFromFileConfigAsync(_parameters);
            }
            else
            {
                config = await ReadFromBlobStorageAsync(_parameters);
            }

            Console.WriteLine("Press control-C to exit.");

            using var cts = new CancellationTokenSource();

            if ( _parameters.ActualDuration > 0)
            {
                cts.CancelAfter(_parameters.ActualDuration * 1000);
            }

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (var device in config.Devices)
            {
                tasks.Add(SendDeviceToCloudMessagesAsync(device, cts.Token));
                await Task.Delay(rand.Next(125, 2000));
            }

            Task.WaitAll(tasks.ToArray());

        }

        private static async Task<DevicesConfiguration> ReadFromBlobStorageAsync(Parameters parameters)
        {
            Console.WriteLine($"Reading configuration from blob '{parameters.BlobUrl}'");

            var uri = new Uri(parameters.BlobUrl);

            using (var client = new HttpClient())
            {
                var configFile = await client.GetStringAsync(uri);
                return JsonSerializer.Deserialize<DevicesConfiguration>(configFile);
            }
        }

        private static async Task<DevicesConfiguration> ReadFromFileConfigAsync(Parameters parameters)
        {
            Console.WriteLine($"Reading configuration from file '{parameters.ConfigFilePath}'");

            var configFile = await File.ReadAllTextAsync(parameters.ConfigFilePath);
            return JsonSerializer.Deserialize<DevicesConfiguration>(configFile);
        }

        private static Random rand = new Random(DateTime.Now.Millisecond);

        private static async Task SendDeviceToCloudMessagesAsync(DeviceConfiguration device, CancellationToken ct)
        {
            var cs = IotHubConnectionStringBuilder.Create(device.ConnectionString);

            var deviceClient = DeviceClient.CreateFromConnectionString(cs.ToString(), TransportType.Mqtt);

            double minTemperature = 20;
            double minHumidity = 60;

            while (!ct.IsCancellationRequested)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetry = new DeviceTelemetry()
                {
                    DeviceId = device.Id,
                    DeviceName = device.Name,
                    Timestamp = DateTimeOffset.Now,
                    Data = new DeviceData()
                    {
                        Humidity = currentHumidity,
                        Temperature = currentTemperature
                    }
                };

                // Create JSON message
                string messageBody = JsonSerializer.Serialize(telemetry);
                using var message = new Message(Encoding.ASCII.GetBytes(messageBody))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                };


                // Send the telemetry message
                await deviceClient.SendEventAsync(message);
                Console.WriteLine($"{DateTime.Now} > Sending message: {messageBody}");

                try
                {
                    await Task.Delay(device.PollingIntervalInSec * 1000, ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            deviceClient.Dispose();
        }
    }
}
