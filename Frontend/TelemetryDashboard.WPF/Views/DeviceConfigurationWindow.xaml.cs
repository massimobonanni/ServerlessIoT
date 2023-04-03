using CommunityToolkit.Mvvm.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TelemetryDashboard.WPF.Interfaces;
using TelemetryDashboard.WPF.Messages;

namespace TelemetryDashboard.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeviceConfigurationWindow.xaml
    /// </summary>
    public partial class DeviceConfigurationWindow : Window, IContextSettableObject
    {
        public DeviceConfigurationWindow()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<CloseWindowMessage>(this, CloseWindowMessageHandler);
        }

        private void CloseWindowMessageHandler(object recipient, CloseWindowMessage msg)
        {
            if (msg.WindowToClose == WindowNames.DeviceConfiguration)
            {
                WeakReferenceMessenger.Default.Unregister<CloseWindowMessage>(this);
                this.Close();
            }
        }

        public async Task<bool> SetContextAsync(object sender, object context, CancellationToken cancellationToken)
        {
            var model = this.DataContext as IContextSettableObject;
            if (model != null)
            {
                return await model.SetContextAsync(this, context, cancellationToken);
            }
            return false;
        }
    }
}
