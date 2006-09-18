using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class ServerViewServerNode : BrowserNode 
    {
        private AEServer _aeserver;
        private AEServerPool _serverPool;

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        internal ServerViewServerNode(AEServer ae, AEServerPool svrPool)
        {
            _aeserver = ae;
            _serverPool = svrPool;
        }

        public override string ServerName
        {
            get
            {
                if (_aeserver.Servername.Equals(AENavigatorComponent.EmptyNodeName))
                    return _aeserver.getServerGroupName();
                return _aeserver.Servername;
            }
        }

        public override string ServerPath
        {
            get
            {
                return _aeserver.Serverpath;
            }
        }

        public override string Details
        {
            get
            {
                if (_aeserver.Servername.Equals(AENavigatorComponent.EmptyNodeName))
                    return "";
                return _aeserver.AEDetails;
            }
        }

        protected override void CreateChildNodes()
        {
            if (!ServerName.Equals(AENavigatorComponent.MyServersTitle))
                return;
            foreach (AEServer ae in _serverPool.Serverlist)
            {
                if (ae.Servername.Equals(AENavigatorComponent.EmptyNodeName) || ae.Servername.Equals(AENavigatorComponent.MyDatastoreTitle))
                    continue;
                AddChild(new ServerViewServerNode(ae, _serverPool));
            }
        }
    }
}
