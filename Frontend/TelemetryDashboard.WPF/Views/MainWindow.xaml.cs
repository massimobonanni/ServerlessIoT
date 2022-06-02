using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelemetryDashboard.WPF.Messages;
using TelemetryDashboard.WPF.Views;

namespace TelemetryDashboard.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<OpenWindowMessage>(this, OpenWindowMessageHandler);
        }

        private async void OpenWindowMessageHandler(OpenWindowMessage msg)
        {
            switch (msg.WindowToOpen)
            {
                case WindowNames.MainWindow:
                    break;
                case WindowNames.DeviceConfiguration:
                    var configWindow = new DeviceConfigurationWindow();
                    configWindow.Show();
                    await configWindow.SetContextAsync(this, msg.Parameter, default);
                    break;
                case WindowNames.DeviceMethod:
                    var methodWindow = new DeviceMethodWindow();
                    methodWindow.Show();
                    await methodWindow.SetContextAsync(this, msg.Parameter, default);
                    break;
                default:
                    break;
            }

        }
    }
}
