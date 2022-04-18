using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryEntities.OpenAPI
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "1.0.0",
            Title = "IoT Serverless Front-End API",
            Description = "",
            TermsOfService = new Uri("https://github.com/massimobonanni/ServerlessIoT"),
            Contact = new OpenApiContact()
            {
                Name = "ServerlessIoT project",
                Url = new Uri("https://github.com/massimobonanni/ServerlessIoT"),
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };
    }
}
