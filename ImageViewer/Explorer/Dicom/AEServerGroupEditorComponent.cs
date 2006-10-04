using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="AEServerGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AEServerGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AEServerGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(AEServerGroupEditorComponentViewExtensionPoint))]
    public class AEServerGroupEditorComponent : ApplicationComponent
    {
        #region Fields

        private AEServerPool _serverPool;
        private string _serverGroupName = "";

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public string ServerGroupName
        {
            get { return _serverGroupName; }
            set 
            { 
                _serverGroupName = value;
                if (_serverGroupName.Equals(""))
                    this.Modified = false;
                else
                    this.Modified = true;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public AEServerGroupEditorComponent(AEServerPool svrPool)
        {
            _serverPool = svrPool;
            if (_serverPool != null)
            {
                if (_serverPool.Currentserver != null)
                {
                    _serverGroupName = _serverPool.Currentserver.getServerGroupName();
                }
                else
                {
                    _serverGroupName = "";
                }
            }

        }

        public void Accept()
        {
            // edit current server group
            if (_serverPool.Currentserver != null && _serverPool.Currentserverid >= 0 && _serverPool.Currentserverid < _serverPool.Serverlist.Count)
            {
                String[] pgroup = _serverPool.Currentserver.ServerPathGroups;
                if (pgroup == null || pgroup.Length <= 0)
                    return;
                String svrpath0 = _serverPool.Currentserver.Serverpath;
                String svrpath = "";
                pgroup[pgroup.Length - 1] = _serverGroupName;
                for (int m = 0; m < pgroup.Length; m++)
                {
                    if (!pgroup[m].Equals(""))
                    {
                        svrpath += "/" + pgroup[m];
                    }
                }
                _serverPool.Currentserver.Serverpath = svrpath;
                _serverPool.Currentserver.ServerPathGroups = pgroup;
                _serverPool.Serverlist[_serverPool.Currentserverid] = _serverPool.Currentserver;
                foreach (AEServer ae in _serverPool.GetChildServers(_serverPool.Currentserverid, false, true))
                {
                    if (ae.Serverpath.StartsWith(svrpath0))
                    {
                        ae.Serverpath = ae.Serverpath.Replace(svrpath0, svrpath);
                        ae.ServerPathGroups = null;
                        _serverPool.Serverlist[ae.ServerID] = ae;
                    }
                }

                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
                return;
            }
            // add new server group
            if (_serverPool.Currentserver == null && _serverPool.Currentserverid >= 0 && _serverPool.Currentserverid < _serverPool.Serverlist.Count)
            {
                _serverPool.Currentserver = _serverPool.Serverlist[_serverPool.Currentserverid];
                AEServer ae = new AEServer(AENavigatorComponent.EmptyNodeName, _serverPool.Currentserver.Serverpath + "/" + _serverGroupName, "", _serverPool.Currentserver.Host, _serverPool.Currentserver.AE, _serverPool.Currentserver.Port);
                ae.ServerParentID = _serverPool.Currentserverid;
                _serverPool.Currentserverid = _serverPool.Serverlist.Count;
                ae.ServerID = _serverPool.Currentserverid;
                ae.ServerPathGroups = null;
                _serverPool.Currentserver = ae;
                _serverPool.Serverlist.Add(_serverPool.Currentserver);
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
                return;
            }
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
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
    }
}
