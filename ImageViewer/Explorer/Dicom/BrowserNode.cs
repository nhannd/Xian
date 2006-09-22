using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class BrowserNode : IBrowserNode
    {
        private List<IBrowserNode> _childNodes;

        protected abstract void CreateChildNodes();

        protected void AddChild(IBrowserNode child)
        {
            _childNodes.Add(child);
        }

        #region IBrowserNode Members

        public List<IBrowserNode> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<IBrowserNode>();
                    CreateChildNodes();
                }
                return _childNodes;
            }
        }

        public abstract string ServerName
        {
            get;
        }

        public abstract int ServerID
        {
            get;
        }

        public abstract bool IsServerNode
        {
            get;
        }
        
        public abstract string ServerPath
        {
            get;
        }

        public abstract string Details
        {
            get;
        }

        #endregion
    }
}
