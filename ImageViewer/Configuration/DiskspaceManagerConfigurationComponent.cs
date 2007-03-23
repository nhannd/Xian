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

            }
            catch (Exception e)
            {
                _driveName = "";
                _status = "";
                _lowWatermark = 0.0F;
                _highWatermark = 0.0F;
                _spaceUsed = 0.0F;
                _serviceClient = null;

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

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set
            {
                _lowWatermark = value;
                this.Modified = true;
            }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set
            {
                _highWatermark = value;
                //NotifyPropertyChanged("HighWatermarkString");
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

        public bool Enabled
        {
            get { return _serviceClient != null; }
        }

        #endregion

    }
}
