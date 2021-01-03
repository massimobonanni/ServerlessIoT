// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
            's',
            "SharedAccessKey",
            HelpText = "A primary or shared access key from your IoT Hub instance, with the 'service' permission. Use `az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}` to fetch via the Azure CLI.")]
        public string SharedAccessKey { get; set; }

        [Option(
            'c',
            "EventHubConnectionString",
            HelpText = "The connection string to the event hub-compatible endpoint. Use the Azure portal to get this parameter. If this value is provided, all the others are not necessary.")]
        public string EventHubConnectionString { get; set; }

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
    }
}
