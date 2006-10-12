using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerGroupEditComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerGroupEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerGroupEditComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerGroupEditComponentViewExtensionPoint))]
    public class DicomServerGroupEditComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerGroupEditComponent(DicomServerTree dicomServerTree)
        {
            _dicomServerTree = dicomServerTree;
            if (!_dicomServerTree.IsMarked && _dicomServerTree.CurrentServer != null)
            {
                _serverGroupName = _dicomServerTree.CurrentServer.ServerName;
            }
            else
            {
                _serverGroupName = "";
            }

        }

        public void Accept()
        {
            if (_dicomServerTree.CurrentServer == null || _dicomServerTree.CurrentServer.IsServer)
            {
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
                Host.Exit();
            }
            // edit current server group
            if (!_dicomServerTree.IsMarked)
            {
                _dicomServerTree.RenameDicomServerGroup((DicomServerGroup)_dicomServerTree.CurrentServer, _serverGroupName, "", "", 0);
            }
            // add new server group
            else
            {
                DicomServerGroup dsg = new DicomServerGroup(_serverGroupName, _dicomServerTree.CurrentServer.GroupID);
                ((DicomServerGroup)_dicomServerTree.CurrentServer).ChildServers.Add(dsg);
                _dicomServerTree.CurrentServer = dsg;
            }
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

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        private bool IsServerSettingValid()
        {
            if (_serverGroupName == null || _serverGroupName.Equals(""))
            {
                return false;
            }

            if (_dicomServerTree.DicomServerNameExists(_dicomServerTree.MyServerGroup, _serverGroupName, _dicomServerTree.CurrentServer.ServerPath, !_dicomServerTree.IsMarked, true))
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Saver Group Name ({0}) exists already.\r\nPlease choose another name.", _serverGroupName);
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                return false;
            }
            return true;
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
        private string _serverGroupName = "";

        public DicomServerTree DicomServerTree
        {
            get { return _dicomServerTree; }
            set { _dicomServerTree = value; }
        }

        public string ServerGroupName
        {
            get { return _serverGroupName; }
            set
            {
                _serverGroupName = value;
                this.Modified = IsServerSettingValid();
            }
        }

        #endregion

    }
}
