// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using CommandLine;

namespace TelemetrySimulator
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class Parameters
    {
        [Option(
            'f',
            "ConfigFile",
            HelpText = "Json File contains the devices configuration")]
        public string ConfigFilePath { get; set; }

        [Option(
            'b',
            "BlobUrl",
            HelpText = "Url for the blob contains JSON configuration for the devices")]
        public string BlobUrl { get; set; }

        [Option(
            'd',
            "Duration",
            HelpText = "Duration of the simulation (in seconds). If you don't set this options, the simuilator runs until you stop it.")]
        public string Duration { get; set; }


        public int ActualDuration
        {
            get
            {
                int.TryParse(Duration, out int actualDuration);
                return actualDuration;
            }
        }
        public bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(BlobUrl) || string.IsNullOrWhiteSpace(ConfigFilePath));
        }

        public bool IsFileConfig()
        {
            return !string.IsNullOrWhiteSpace(ConfigFilePath);
        }

    }
}
