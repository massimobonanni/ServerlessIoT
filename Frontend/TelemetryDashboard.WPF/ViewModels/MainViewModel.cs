using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ServerlessIoT.Core.Interfaces;
using ServerlessIoT.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TelemetryDashboard.WPF.ViewModels
{

    public class MainViewModel : ViewModelBase
    {

        private readonly BackgroundWorker _telemetryUpdateWorker;
        private readonly ITelemetryManager _telemetryManager;

        public MainViewModel(ITelemetryManager telemetryManager)
        {
            this._telemetryManager = telemetryManager;

            _telemetryUpdateWorker = new BackgroundWorker();
            _telemetryUpdateWorker.WorkerSupportsCancellation = true;
            _telemetryUpdateWorker.DoWork += TelemetryUpdateWorker_DoWork;
        }

        private async void TelemetryUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                this.BackgroundWorkerRunning = true;
            });

            while (!worker.CancellationPending)
            {
                var devices = await this._telemetryManager.GetDevicesAsync(default);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        if (devices != null)
                        {
                            this.Devices = new ObservableCollection<DeviceInfoModel>(
                                devices.OrderBy(d => d.DeviceId));
                        }
                        else
                        {
                            this.Devices = null;
                        }
                    }
                );
                await Task.Delay(5000);
            }

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                this.BackgroundWorkerRunning = false;
            });
        }

        private ObservableCollection<DeviceInfoModel> _devices;
        public ObservableCollection<DeviceInfoModel> Devices
        {
            get
            {
                return _devices;
            }
            set
            {
                _devices = value;
                this.RaisePropertyChanged();
            }
        }

        private string _apiURL;
        public string ApiURL
        {
            get
            {
                return _apiURL;
            }
            set
            {
                _apiURL = value;
                this.RaisePropertyChanged();
            }
        }

        private bool _backgroundWorkerRunning;
        public bool BackgroundWorkerRunning
        {
            get
            {
                return _backgroundWorkerRunning;
            }
            set
            {
                _backgroundWorkerRunning = value;
                this.RaisePropertyChanged();
                this.StartTelemetryVisualization.RaiseCanExecuteChanged();
                this.StopTelemetryVisualization.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand _startTelemetryVisualization;
        public RelayCommand StartTelemetryVisualization
        {
            get
            {
                if (_startTelemetryVisualization == null)
                {
                    _startTelemetryVisualization = new RelayCommand(
                        () =>
                        {
                            this._telemetryUpdateWorker.RunWorkerAsync();
                        },
                        () =>
                        {
                            return !this.BackgroundWorkerRunning;
                        });
                }
                return _startTelemetryVisualization;
            }
        }

        private RelayCommand _stopTelemetryVisualization;
        public RelayCommand StopTelemetryVisualization
        {
            get
            {
                if (_stopTelemetryVisualization == null)
                {
                    _stopTelemetryVisualization = new RelayCommand(
                        () =>
                        {
                            this._telemetryUpdateWorker.CancelAsync();
                        },
                        () =>
                        {
                            return this.BackgroundWorkerRunning;
                        });
                }
                return _stopTelemetryVisualization;
            }
        }

    }
}