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
            "APIUrl",
            HelpText = "Url for the Devices APIs")]
        public string APIUrl { get; set; }

        [Option(
            'k',
            "APIKey",
            HelpText = "API Key for the Devices APIs")]
        public string APIKey { get; set; }

    }
}
