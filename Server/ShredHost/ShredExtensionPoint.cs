using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    [ExtensionPoint()]
    public class ShredExtensionPoint : ExtensionPoint<IShred>
    {
    }
}
