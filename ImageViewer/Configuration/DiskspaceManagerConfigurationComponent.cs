using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DiskspaceManagerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DiskspaceManagerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _driveName;
        private string _status;
        private float _lowWatermark;
        private float _highWatermark;
        private float _spaceUsed;
        private float _watermarkMinDifference = 5.0F;
        private int _checkFrequency;

        private DiskspaceManagerServiceClient _serviceClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponent()
        {
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            ConnectToClient();
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public void ConnectToClient()
        {
			BlockingOperation.Run(this.ConnectToClientInternal);
			SignalPropertyChanged();
		}

		private void ConnectToClientInternal()
		{
			try
			{
				_serviceClient = new DiskspaceManagerServiceClient();
				GetServerSettingResponse response = _serviceClient.GetServerSetting();
				_serviceClient.Close();

				_driveName = response.DriveName;
				_status = response.Status;
				_lowWatermark = response.LowWatermark;
				_highWatermark = response.HighWatermark;
				_spaceUsed = response.UsedSpace;
				_checkFrequency = response.CheckFrequency;
			}
			catch
			{
				_driveName = "";
				_status = "";
				_lowWatermark = 0.0F;
				_highWatermark = 0.0F;
				_spaceUsed = 0.0F;
				_serviceClient = null;
				_checkFrequency = 10;

				this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToRetrieveDiskspaceManagementSettings, MessageBoxActions.Ok);
			}
		}

        public override void Save()
        {
            if (_serviceClient != null)
            {
                try
                {
                    UpdateServerSettingRequest request = new UpdateServerSettingRequest();
                    request.DriveName = _driveName;
                    request.Status = _status;
                    request.LowWatermark = _lowWatermark;
                    request.HighWatermark = _highWatermark;
                    request.UsedSpace = _spaceUsed;
                    request.CheckFrequency = _checkFrequency;

                    _serviceClient = new DiskspaceManagerServiceClient();
                    _serviceClient.UpdateServerSetting(request);
                    _serviceClient.Close();
                }
                catch
                {
					this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToUpdateDiskspaceManagementSettings, MessageBoxActions.Ok);
                }
            }
        }

        private void SignalPropertyChanged()
        {
            NotifyPropertyChanged("DriveName");
            NotifyPropertyChanged("Status");
            NotifyPropertyChanged("LowWatermark");
            NotifyPropertyChanged("HighWatermark");
            NotifyPropertyChanged("UsedSpace");
            NotifyPropertyChanged("Enabled");
        }

        #region Properties

        public string DriveName
        {
            get { return _driveName; }
            set
            {
                _driveName = value;
                this.Modified = true;
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                this.Modified = true;
            }
        }

        public float WatermarkMinDifference
        {
            get { return _watermarkMinDifference; }
        }

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set
            {
                if (value >= (100.0F - _watermarkMinDifference))
                {
                    SignalPropertyChanged();
                    _lowWatermark = 100.0F - _watermarkMinDifference;
                }
                else if (value <= 0.0F)
                {
                    SignalPropertyChanged();
                    _lowWatermark = 0.0F;
                }
                else
                    _lowWatermark = value;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                {
                    SignalPropertyChanged();
                    _highWatermark = _lowWatermark + _watermarkMinDifference;
                }
                this.Modified = true;
            }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set
            {
                if (value >= 100.0F)
                {
                    SignalPropertyChanged();
                    _highWatermark = 100.0F;
                }
                else if (value <= _watermarkMinDifference)
                {
                    SignalPropertyChanged();
                    _highWatermark = _watermarkMinDifference;
                }
                else
                    _highWatermark = value;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                {
                    SignalPropertyChanged();
                    _lowWatermark = _highWatermark - _watermarkMinDifference;
                }
                this.Modified = true;
            }
        }

        public float SpaceUsed
        {
            get { return _spaceUsed; }
            set
            {
                _spaceUsed = value;
                this.Modified = true;
            }
        }

        public float LowWatermarkDisplay
        {
            get { return (_lowWatermark * 100.0F); }
            set
            {
                if (value >= (100.0F - _watermarkMinDifference) * 100.0F)
                {
                    SignalPropertyChanged();
                    _lowWatermark = (100.0F - _watermarkMinDifference);
                }
                else if (value <= 0.0F)
                {
                    SignalPropertyChanged();
                    _lowWatermark = 0.0F;
                }
                else
                    _lowWatermark = value / 100.0F;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                {
                    SignalPropertyChanged();
                    _highWatermark = _lowWatermark + _watermarkMinDifference;
                }
                this.Modified = true;
            }
        }

        public float HighWatermarkDisplay
        {
            get { return (_highWatermark * 100.0F); }
            set
            {
                if (value >= 10000.0F)
                {
                    SignalPropertyChanged();
                    _highWatermark = 100.0F;
                }
                else if (value <= (_watermarkMinDifference * 100.0F))
                {
                    SignalPropertyChanged();
                    _highWatermark = _watermarkMinDifference;
                }
                else
                    _highWatermark = value / 100.0F;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                {
                    SignalPropertyChanged();
                    _lowWatermark = _highWatermark - _watermarkMinDifference;
                }
                this.Modified = true;
            }
        }

        public int CheckFrequency
        {
            get { return _checkFrequency; }
            set
            {
                _checkFrequency = value;
                this.Modified = true;
            }
        }

        public bool Enabled
        {
            get { return _serviceClient != null && !DriveName.Equals(""); }
        }

        #endregion

    }
}
