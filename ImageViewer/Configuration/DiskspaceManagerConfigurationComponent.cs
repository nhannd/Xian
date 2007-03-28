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
        private List<string> _availableDrives;
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
            try
            {
                if (_serviceClient != null)
                    _serviceClient.Close();

                _serviceClient = new DiskspaceManagerServiceClient();
                GetServerSettingResponse response = _serviceClient.GetServerSetting();

                _availableDrives = new List<string>();
                _driveName = response.DriveName;
                _availableDrives.Add(_driveName);
                _status = response.Status;
                _lowWatermark = response.LowWatermark;
                _highWatermark = response.HighWatermark;
                _spaceUsed = response.UsedSpace;
                _checkFrequency = response.CheckFrequency;

            }
            catch (Exception e)
            {
                _driveName = "";
                _status = "";
                _lowWatermark = 0.0F;
                _highWatermark = 0.0F;
                _spaceUsed = 0.0F;
                _serviceClient = null;
                _checkFrequency = 10;

                ExceptionHandler.Report(e, this.Host.DesktopWindow);
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
                    _serviceClient.UpdateServerSetting(request);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionFailedToUpdateServerSettings, this.Host.DesktopWindow);
                }
            }
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

        public List<string> AvailableDrives
        {
            get { return _availableDrives; }
            set
            {
                _availableDrives = value;
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
                    _lowWatermark = 100.0F - _watermarkMinDifference;
                else if (value <= 0.0F)
                    _lowWatermark = 0.0F;
                else
                    _lowWatermark = value;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                    _highWatermark = _lowWatermark + _watermarkMinDifference;
                this.Modified = true;
            }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set
            {
                if (value >= 100.0F)
                    _highWatermark = 100.0F;
                else if (value <= _watermarkMinDifference)
                    _highWatermark = _watermarkMinDifference;
                else
                    _highWatermark = value;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                    _lowWatermark = _highWatermark - _watermarkMinDifference;
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
                    _lowWatermark = (100.0F - _watermarkMinDifference);
                else if (value <= 0.0F)
                    _lowWatermark = 0.0F;
                else
                    _lowWatermark = value / 100.0F;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                    _highWatermark = _lowWatermark + _watermarkMinDifference;
                this.Modified = true;
            }
        }

        public float HighWatermarkDisplay
        {
            get { return (_highWatermark * 100.0F); }
            set
            {
                if (value >= 10000.0F)
                    _highWatermark = 100.0F;
                else if (value <= (_watermarkMinDifference * 100.0F))
                    _highWatermark = _watermarkMinDifference;
                else
                    _highWatermark = value / 100.0F;
                if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
                    _lowWatermark = _highWatermark - _watermarkMinDifference;
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
            get { return _serviceClient != null; }
        }

        #endregion

    }
}
