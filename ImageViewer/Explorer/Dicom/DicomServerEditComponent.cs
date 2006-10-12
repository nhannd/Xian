using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerEditComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerEditComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerEditComponentViewExtensionPoint))]
    public class DicomServerEditComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerEditComponent(DicomServerTree dicomServerTree)
        {
            _dicomServerTree = dicomServerTree;
            if (_dicomServerTree.CurrentServer == null)
                return;
            if (_dicomServerTree.CurrentServer.IsServer)
            {
                _serverName = _dicomServerTree.CurrentServer.ServerName;
                _serverPath = _dicomServerTree.CurrentServer.ServerPath;
                _serverLocation = ((DicomServer)_dicomServerTree.CurrentServer).ServerLocation;
                _serverAE = ((DicomServer)_dicomServerTree.CurrentServer).DicomAE.AE;
                _serverHost = ((DicomServer)_dicomServerTree.CurrentServer).DicomAE.Host;
                _serverPort = ((DicomServer)_dicomServerTree.CurrentServer).DicomAE.Port.ToString();
            }
            else
            {
                _serverName = "";
                _serverPath = _dicomServerTree.CurrentServer.GroupID;
                _serverLocation = "";
                _serverAE = "";
                _serverHost = "";
                _serverPort = "";
            }
        }

        public void Accept()
        {
            if (_dicomServerTree.CurrentServer == null)
            {
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
                Host.Exit();
                return;
            }
            DicomServer ds = new DicomServer(_serverName, _serverPath, _serverLocation, _serverHost, _serverAE, int.Parse(_serverPort));
            // edit current server
            if (_dicomServerTree.CurrentServer.IsServer)
            {
                _dicomServerTree.ReplaceDicomServer((DicomServer)_dicomServerTree.CurrentServer, ds);
            }
            // add new server
            else
            {
                ((DicomServerGroup)_dicomServerTree.CurrentServer).AddChild(ds);
            }
            _dicomServerTree.CurrentServer = ds;
            _dicomServerTree.SaveDicomServers();
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
            return;
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private bool IsServerSettingValid()
        {
            if (_serverName == null || _serverName.Equals("") || _serverAE == null || _serverAE.Equals("")
                || _serverHost == null || _serverHost.Equals("") || _serverPort == null || _serverPort.Equals(""))
            {
                return false;
            }

            if (_dicomServerTree.DicomServerNameExists(_dicomServerTree.MyServerGroup, _serverName, _dicomServerTree.CurrentServer.ServerPath, _dicomServerTree.CurrentServer.IsServer, true))
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Saver Name ({0}) exists already.\r\nPlease choose another server name.", _serverName);
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                return false;
            }
            if (_dicomServerTree.DicomServerAEExists(_dicomServerTree.MyServerGroup, _serverAE, _dicomServerTree.CurrentServer.ServerPath, _dicomServerTree.CurrentServer.IsServer, true))
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The AE Title ({0}) exists already.\r\nPlease choose another AE Title.", _serverAE);
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                return false;
            }

            try
            {
                int.Parse(_serverPort);
            }
            catch
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Port value should be a integer. \r\n\r\nPlease input an integer data.");
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                return false;
            }

            return true;
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public bool FieldReadonly
        {
            get 
            {
                return _serverName.Equals(AENavigatorComponent.MyDatastoreTitle)? true : false; 
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Fields

        private DicomServerTree _dicomServerTree;
        private string _serverName = "";
        private string _serverPath = "";
        private string _serverLocation = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";

        public DicomServerTree DicomServerTree
        {
          get { return _dicomServerTree; }
          set { _dicomServerTree = value; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set 
            { 
                _serverName = value;
                this.Modified = IsServerSettingValid();
            }
        }

        public string ServerLocation
        {
            get { return _serverLocation; }
            set { 
                _serverLocation = value;
                this.Modified = true;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { 
                _serverAE = value;
                this.Modified = IsServerSettingValid();
            }
        }

        public string ServerHost
        {
            get { return _serverHost; }
            set { 
                _serverHost = value;
                this.Modified = IsServerSettingValid();
            }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { 
                _serverPort = value;
                this.Modified = IsServerSettingValid();
            }
        }

        #endregion

    }

}
