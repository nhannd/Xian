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
        public DicomServerGroupEditComponent(DicomServerTree dicomServerTree, bool isNewServerGroup)
        {
            _isNewServerGroup = isNewServerGroup;
            _dicomServerTree = dicomServerTree;
            if (!_isNewServerGroup)
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
            if (!IsServerGroupNameValid())
                return;
            if (!_isNewServerGroup)
            {
                _dicomServerTree.RenameDicomServerGroup((DicomServerGroup)_dicomServerTree.CurrentServer, _serverGroupName, "", "", 0);
                if (((DicomServerGroup)_dicomServerTree.CurrentServer).ChildServers.Count > 0)
                    _dicomServerTree.FireServerTreeUpdatedEvent();
            }
            else
            {
                DicomServerGroup dsg = new DicomServerGroup(_serverGroupName, _dicomServerTree.CurrentServer.ServerPath + "/" + _dicomServerTree.CurrentServer.ServerName);
                ((DicomServerGroup)_dicomServerTree.CurrentServer).AddChild(dsg);
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

        private bool IsServerGroupNameEmpty()
        {
            if (_serverGroupName == null || _serverGroupName.Equals("")
                || !_isNewServerGroup && _serverGroupName.Equals(_dicomServerTree.CurrentServer.ServerName))
            {
                return false;
            }

            return true;
        }

        private bool IsServerGroupNameValid()
        {
            string msg = _dicomServerTree.DicomServerGroupNameValidation(_serverGroupName);
            if (!msg.Equals(""))
            {
                this.Modified = false;
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Saver Group Name ({0}) is conflict with {1}.\r\nPlease choose another name.", _serverGroupName, msg);
                throw new DicomServerException(msgText.ToString());
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
        private bool _isNewServerGroup;

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
                this.Modified = IsServerGroupNameEmpty();
            }
        }

        #endregion

    }
}
