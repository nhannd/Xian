using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Shreds.DiskspaceManager
{
	public partial class DiskspaceManagerProcessor : IDiskspaceManagerService
    {
		private static DiskspaceManagerProcessor _instance;

        private string _driveName;
        private string _status;
        private float _lowWatermark;
        private float _highWatermark;
        private float _usedSpace;

        public DiskspaceManagerProcessor()
        {
            _driveName = DiskspaceManagerSettings.Instance.DriveName;
            _status = DiskspaceManagerSettings.Instance.Status;
            _lowWatermark = DiskspaceManagerSettings.Instance.LowWatermark;
            _highWatermark = DiskspaceManagerSettings.Instance.HighWatermark;
            _usedSpace = DiskspaceManagerSettings.Instance.UsedSpace;

        }

        public static DiskspaceManagerProcessor Instance
		{
			get
			{
				if (_instance == null)
                    _instance = new DiskspaceManagerProcessor();

				return _instance;
			}
            set
            {
                _instance = value;
            }
		}

        public void StartProcessor()
        {
            try
            {
                _driveName = "E";
                _status = "Start ...";
                _lowWatermark = 55.5F;
                _highWatermark = 88.8F;
                _usedSpace = 77.7F;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StopProcessor()
        {
            try
            {
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region Properties

        public string DriveName
        {
            get { return _driveName; }
            set { _driveName = value; }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set { _lowWatermark = value; }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set { _highWatermark = value; }
        }

        public float UsedSpace
        {
            get { return _usedSpace; }
            set { _usedSpace = value; }
        }

        #endregion

        #region IDiskspaceManagerService Members

        public GetServerSettingResponse GetServerSetting()
        {
            return new GetServerSettingResponse(DiskspaceManagerSettings.Instance.DriveName,
                                                DiskspaceManagerSettings.Instance.Status,
                                                DiskspaceManagerSettings.Instance.LowWatermark,
                                                DiskspaceManagerSettings.Instance.HighWatermark,
                                                DiskspaceManagerSettings.Instance.UsedSpace);
        }

        public void UpdateServerSetting(UpdateServerSettingRequest request)
        {
            DiskspaceManagerSettings.Instance.DriveName = request.DriveName;
            DiskspaceManagerSettings.Instance.Status = request.Status;
            DiskspaceManagerSettings.Instance.LowWatermark = request.LowWatermark;
            DiskspaceManagerSettings.Instance.HighWatermark = request.HighWatermark;
            DiskspaceManagerSettings.Instance.UsedSpace = request.UsedSpace;
            DiskspaceManagerSettings.Save();
        }

		#endregion

	}
}
