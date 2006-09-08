using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;


namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class ServerViewRootNode : BrowserNode
    {
        private AEServerPool _serverPool;

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public ServerViewRootNode(AEServerPool svrPool)
        {
            _serverPool = svrPool;
        }

        protected override void CreateChildNodes()
        {
            if (_serverPool == null)
                return;
            foreach (AEServer ae in _serverPool.Serverlist)
            {
                AddChild(new ServerViewServerNode(ae));
            }
        }

        public override string DisplayName
        {
            get { return AENavigatorComponent.MyServersTitle; }
        }

        public override string Details
        {
            get { return ""; }
        }
    }
}
