﻿using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelemetryDashboard.WPF.Interfaces;
using TelemetryDashboard.WPF.Messages;
using TelemetryEntities.Models;

namespace TelemetryDashboard.WPF.ViewModels
{
    public class DeviceMethodViewModel : ViewModelBase, IInitializableObject, IContextSettableObject
    {
        private readonly IDeviceManager _deviceManager;

        public DeviceMethodViewModel(IDeviceManager deviceManager)
        {
            this._deviceManager = deviceManager;
        }

        #region [ Properties ]
        public string ViewTitle
        {
            get => $"Call Device Method - [{this.DeviceName}]";
        }

        private string _deviceName;
        public string DeviceName
        {
            get => this._deviceName;
            set
            {
                _deviceName = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(ViewTitle));
            }
        }

        private string _deviceId;
        public string DeviceId
        {
            get => this._deviceId;
            set
            {
                _deviceId = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(ViewTitle));
            }
        }

        private string _methodName;
        public string MethodName
        {
            get => this._methodName;
            set
            {
                _methodName = value;
                this.RaisePropertyChanged();
            }
        }

        private string _methodPayload;
        public string MethodPayload
        {
            get => this._methodPayload;
            set
            {
                _methodPayload = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion [ Properties ]

        #region [ Commands ]
        private RelayCommand _callDeviceMethodCommand;
        public RelayCommand CallDeviceMethodCommand
        {
            get
            {
                if (_callDeviceMethodCommand == null)
                {
                    _callDeviceMethodCommand = new RelayCommand(
                        async () => await CallDeviceMethodAsync());
                }
                return _callDeviceMethodCommand;
            }
        }
        #endregion [ Commands ]

        private async Task CallDeviceMethodAsync()
        {
            this.IsBusy = true;

            if (string.IsNullOrEmpty(this.MethodName))
                return;

            var method = new DeviceMethodModel()
            {
               Method=this.MethodName,
               Payload=this.MethodPayload
            };

            var updateResult = await this._deviceManager.CallDeviceMethodAsync(this.DeviceId, method, default);

            this.IsBusy = false;
            if (updateResult)
            {
                Messenger.Default.Send(new CloseWindowMessage()
                {
                    WindowToClose = WindowNames.DeviceMethod,
                    Parameter = null
                });
            }
        }

        public Task InitializeAsync(object sender, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public  Task<bool> SetContextAsync(object sender, object context, CancellationToken cancellationToken)
        {
            var device = context as DeviceInfoModel;
            if (device != null)
            {
                this.DeviceName = device.DeviceName;
                this.DeviceId = device.DeviceId;
               
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
