using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TelemetryEntities;
using TelemetryEntities.Services;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TelemetryEntities
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IEntityFactory, EntityFactory>();
        }
    }
}
