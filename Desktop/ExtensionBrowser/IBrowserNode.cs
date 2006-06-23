using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workstation.ExtensionBrowser
{
    public interface IBrowserNode
    {
        List<IBrowserNode> ChildNodes
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        string Details
        {
            get;
        }
    }
}
