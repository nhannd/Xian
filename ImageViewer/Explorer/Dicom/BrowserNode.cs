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

        /*protected string GetDefaultDisplayName(IBrowsable browsableObject)
        {
            // for now, return the formal name of the object
            // in future, might use some logic to determine whether to use the formal
            // name or the friendly name
            return browsableObject.FormalName;
        }

        protected string GetDefaultDetails(IBrowsable browsableObject)
        {
            // for now, return the description of the object
            // although a given node class may wish to provide other details
            return browsableObject.Description == null ? "" : browsableObject.Description;
        }*/

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

        public abstract string DisplayName
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
