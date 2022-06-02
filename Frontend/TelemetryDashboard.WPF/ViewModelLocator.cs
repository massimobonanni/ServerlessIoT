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
                var httpClient = SimpleIoc.Default.GetInstance<HttpClient>();
                return new DeviceManagerRestProvider(httpClient, App.Parameters.APIUrl, App.Parameters.APIKey);
            });

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DeviceConfigurationViewModel>();
            SimpleIoc.Default.Register<DeviceMethodViewModel>();
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public DeviceConfigurationViewModel DeviceConfigurationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DeviceConfigurationViewModel>();
            }
        }


        public DeviceMethodViewModel DeviceMethodViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DeviceMethodViewModel>();
            }
        }
    }
}
