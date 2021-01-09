using CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace TelemetryDashboard.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        internal static Parameters Parameters;

        protected override void OnStartup(StartupEventArgs e)
        {
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(e.Args)
                .WithParsed(parsedParams =>
                {
                    Parameters = parsedParams;
                    if (string.IsNullOrWhiteSpace(Parameters.APIUrl))
                        Parameters.APIUrl = "http://localhost:7071";
                })
                .WithNotParsed(errors =>
                {
                    Parameters = new Parameters() { APIUrl = "http://localhost:7071" };
                });

            base.OnStartup(e);

            var startupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localLicFile = Path.Combine(startupPath, "license.local.txt");
            if (File.Exists(localLicFile))
            {
                var licenseNumber = File.ReadAllText(localLicFile);
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseNumber);
            }

        }
    }
}
