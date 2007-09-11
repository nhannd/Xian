using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface IExtensionFactory
    {
        object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne);

        ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter);
    }
}
