using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Shreds.DicomServer;

namespace ClearCanvas.ImageViewer.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _hostName;
        private string _aeTitle;
        private int _port;
        private string _storageDir;

        private DicomMoveRequestServiceClient _serviceClient;
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponent()
        {
        }

        public override void Start()
        {
            try
            {
                _serviceClient = new DicomMoveRequestServiceClient();
                GetServerSettingResponse response = _serviceClient.GetServerSetting();
                _hostName = response.HostName;
                _aeTitle = response.AETitle;
                _port = response.Port;
                _storageDir = response.InterimStorageDirectory;
            }
            catch (Exception e)
            {
                //TODO: handle exception
            }

            base.Start();
        }

        public override void Stop()
        {
            // Always close the client.
            if (_serviceClient != null)
            {
                _serviceClient.Close();
            }

            base.Stop();
        }

        public override void Save()
        {
            if (_serviceClient != null)
            {
                UpdateServerSettingRequest request = new UpdateServerSettingRequest();
                request.HostName = _hostName;
                request.AETitle = _aeTitle;
                request.Port = _port;
                request.InterimStorageDirectory = _storageDir;
                _serviceClient.UpdateServerSetting(request);
            }
        }

        #region Properties

        public string HostName
        {
            get { return _hostName; }
            set 
            { 
                _hostName = value;
                this.Modified = true;
            }
        }

        public string AETitle
        {
            get { return _aeTitle; }
            set 
            { 
                _aeTitle = value;
                this.Modified = true;
            }
        }

        public int Port
        {
            get { return _port; }
            set 
            { 
                _port = value;
                this.Modified = true;
            }
        }

        public string StorageDir
        {
            get { return _storageDir; }
            set 
            { 
                _storageDir = value;
                this.Modified = true;
            }
        }

        #endregion

    }
}
