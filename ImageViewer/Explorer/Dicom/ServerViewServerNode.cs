using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class ServerViewServerNode : BrowserNode 
    {
        private AEServer _aeserver;

        internal ServerViewServerNode(AEServer ae)
        {
            _aeserver = ae;
        }

        public override string DisplayName
        {
            get
            {
                return _aeserver.Servername;
            }
        }

        public override string Details
        {
            get
            {
                return _aeserver.Description;
            }
        }

        protected override void CreateChildNodes()
        {
            int i = 0;
            //AddChild(new PluginViewExtensionPointsNode(_plugin));
            //AddChild(new PluginViewExtensionsNode(_plugin));
        }
    }
}
