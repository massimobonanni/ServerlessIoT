using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TelemetryDashboard.WPF.Interfaces;
using TelemetryDashboard.WPF.Messages;

namespace TelemetryDashboard.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeviceMethodWindow.xaml
    /// </summary>
    public partial class DeviceMethodWindow : Window, IContextSettableObject
    {
        public DeviceMethodWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<CloseWindowMessage>(this, CloseWindowMessageHandler);
        }

        private void CloseWindowMessageHandler(CloseWindowMessage msg)
        {
            if (msg.WindowToClose == WindowNames.DeviceMethod)
            {
                Messenger.Default.Unregister<CloseWindowMessage>(this, CloseWindowMessageHandler);
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
