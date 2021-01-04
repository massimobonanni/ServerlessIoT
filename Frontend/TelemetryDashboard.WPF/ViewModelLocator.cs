using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => new HttpClient());
            SimpleIoc.Default.Register<IDeviceManager>(() =>
            {
                string baseUrl = "http://localhost:7071";
                string apiKey = null;
                var httpClient = SimpleIoc.Default.GetInstance<HttpClient>();
                return new DeviceManagerRestProvider(httpClient, baseUrl, apiKey);
            });

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

    }
}
