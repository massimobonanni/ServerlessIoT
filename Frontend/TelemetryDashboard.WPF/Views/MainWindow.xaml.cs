using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
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

            WeakReferenceMessenger.Default.Register<OpenWindowMessage>(this, OpenWindowMessageHandler);
        }

        private async void OpenWindowMessageHandler(object recipient, OpenWindowMessage msg)
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
