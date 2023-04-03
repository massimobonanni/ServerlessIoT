using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ServerlessIoT.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TelemetryDashboard.WPF.ViewModels;
using TelemetryEntities.Rest;

namespace TelemetryDashboard.WPF
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddScoped<HttpClient>(sp => new HttpClient())
                    .AddScoped<IDeviceManager>(sp =>
                    {
                        var httpClient = sp.GetService<HttpClient>();
                        return new DeviceManagerRestProvider(httpClient, App.Parameters.APIUrl, App.Parameters.APIKey);
                    })
                    .AddScoped<MainViewModel>()
                    .AddScoped<DeviceConfigurationViewModel>()
                    .AddScoped<DeviceMethodViewModel>()
                    .BuildServiceProvider()
            );
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return Ioc.Default.GetService<MainViewModel>();
            }
        }

        public DeviceConfigurationViewModel DeviceConfigurationViewModel
        {
            get
            {
                return Ioc.Default.GetService<DeviceConfigurationViewModel>();
            }
        }


        public DeviceMethodViewModel DeviceMethodViewModel
        {
            get
            {
                return Ioc.Default.GetService<DeviceMethodViewModel>();
            }
        }
    }
}
