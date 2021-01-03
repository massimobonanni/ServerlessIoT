using CommandLine;
using System;

namespace TelemetryDispatcher
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class Parameters
    {
        internal const string IotHubSharedAccessKeyName = "service";

        [Option(
            'e',
            "EventHubCompatibleEndpoint",
            HelpText = "The event hub-compatible endpoint from your IoT Hub instance. Use `az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}` to fetch via the Azure CLI.")]
        public string EventHubCompatibleEndpoint { get; set; }

        [Option(
            'n',
            "EventHubName",
            HelpText = "The event hub-compatible name of your IoT Hub instance. Use `az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}` to fetch via the Azure CLI.")]
        public string EventHubName { get; set; }

        [Option(
            'a',
            "SharedAccessKey",
            HelpText = "A primary or shared access key from your IoT Hub instance, with the 'service' permission. Use `az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}` to fetch via the Azure CLI.")]
        public string SharedAccessKey { get; set; }

        [Option(
            'c',
            "EventHubConnectionString",
            HelpText = "The connection string to the event hub-compatible endpoint. Use the Azure portal to get this parameter. If this value is provided, all the others are not necessary.")]
        public string EventHubConnectionString { get; set; }

        [Option(
            's',
            "StorageConnectionString",
            HelpText = "The storage connection string uses by the dispatcher to store the offset for messages retrieved from the iot hub.")]
        public string StorageConnectionString { get; set; }

        [Option(
            'b',
            "BlobContainerName",
            HelpText = "The blob container name uses by the dispatcher to store the offset for messages retrieved from the iot hub.")]
        public string BlobContainerName { get; set; }

        [Option(
            'g',
            "ConsumerGroup",
            HelpText = "The consumer group name uses by the dispatcher connect to the iot hub. The default value is '$Default'.",
            Default ="$Default")]
        public string ConsumerGroupName { get; set; }

        [Option(
            'u',
            "EntitiesBaseAPIUrl",
            HelpText = "The base URL for the Device Entities API.")]
        public string EntitiesAPIUrl { get; set; }

        [Option(
            'k',
            "EntitiesAPIKey",
            HelpText = "The API Key for the Device Entities API.")]
        public string EntitiesAPIKey { get; set; }

        internal string GetEventHubConnectionString()
        {
            return EventHubConnectionString ?? $"Endpoint={EventHubCompatibleEndpoint};SharedAccessKeyName={IotHubSharedAccessKeyName};SharedAccessKey={SharedAccessKey}";
        }

        internal bool IsAPIEndpointEnabled()
        {
            return !string.IsNullOrWhiteSpace(EntitiesAPIUrl) &&
                Uri.IsWellFormedUriString(EntitiesAPIUrl, UriKind.Absolute);
        }

        internal bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(EventHubConnectionString)
                && (string.IsNullOrWhiteSpace(EventHubCompatibleEndpoint)
                    || string.IsNullOrWhiteSpace(EventHubName)
                    || string.IsNullOrWhiteSpace(SharedAccessKey))
                && (string.IsNullOrWhiteSpace(StorageConnectionString) && string.IsNullOrWhiteSpace(BlobContainerName)))
            {
                return false;
            }
            return true;
        }

    }
}
