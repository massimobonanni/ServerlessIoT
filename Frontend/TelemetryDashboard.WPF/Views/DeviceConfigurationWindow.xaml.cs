﻿using System;
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

namespace TelemetryDashboard.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeviceConfigurationWindow.xaml
    /// </summary>
    public partial class DeviceConfigurationWindow : Window,IContextSettableObject
    {
        public DeviceConfigurationWindow()
        {
            InitializeComponent();
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
