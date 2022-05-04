// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace TelemetryDashboard.WPF
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class Parameters
    {
        [Option(
            'u',
            "apiurl",
            HelpText = "Url for the Devices APIs")]
        public string APIUrl { get; set; }

        [Option(
            'k',
            "apikey",
            HelpText = "API Key for the Devices APIs")]
        public string APIKey { get; set; }

        [Option(
            'p',
            "pollingtime",
            HelpText = "Polling time (in secs) between each call to retrieve telemetry for a device",
            Default = 10,
            Required = false)]
        public int PollingTimeInSec { get; set; }

    }
}
