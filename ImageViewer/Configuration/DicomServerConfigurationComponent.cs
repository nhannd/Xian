using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Services.DicomServer;

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

        private DicomServerServiceClient _serviceClient;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponent()
        {
        }

        public override void Start()
        {
            ConnectToClient();
            base.Start();
        }

        public override void Stop()
        {
            // Always close the client.
            if (_serviceClient != null)
            {
                _serviceClient.Close();
                _serviceClient = null;
            }

            base.Stop();
        }

        public void ConnectToClient()
        {
            try
            {
                BackgroundTask task = new BackgroundTask(delegate(IBackgroundTaskContext backgroundcontext)
                    {
                        backgroundcontext.ReportProgress(new BackgroundTaskProgress(0, SR.MessageRetrievingServerSettings));

                        try
                        {
                            if (_serviceClient != null)
                                _serviceClient.Close();

                            _serviceClient = new DicomServerServiceClient();
                            GetServerSettingResponse response = _serviceClient.GetServerSetting();

                            _hostName = response.HostName;
                            _aeTitle = response.AETitle;
                            _port = response.Port;
                            _storageDir = response.InterimStorageDirectory;

                            backgroundcontext.Complete(null);
                        }
                        catch (Exception e)
                        {
                            _hostName = "";
                            _aeTitle = "";
                            _port = 0;
                            _storageDir = "";
                            _serviceClient = null;

                            backgroundcontext.ReportProgress(new BackgroundTaskProgress(100, SR.ExceptionFailedToRetrieveServerSettings));
                            backgroundcontext.Error(e);
                        }
                    }, false);

                ProgressDialog.Show(task, true, ProgressBarStyle.Marquee, this.Host.DesktopWindow);
                SignalPropertyChanged();
            }
            catch (Exception e)
            {
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
                    request.HostName = _hostName;
                    request.AETitle = _aeTitle;
                    request.Port = _port;
                    request.InterimStorageDirectory = _storageDir;
                    _serviceClient.UpdateServerSetting(request);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionFailedToUpdateServerSettings, this.Host.DesktopWindow);
                }
            }
        }

        private void SignalPropertyChanged()
        {
            NotifyPropertyChanged("HostName");
            NotifyPropertyChanged("AETitle");
            NotifyPropertyChanged("Port");
            NotifyPropertyChanged("InterimStorageDirectory");
            NotifyPropertyChanged("Enabled");
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

        public bool Enabled
        {
            get { return _serviceClient != null; }
        }

        #endregion

    }
}
