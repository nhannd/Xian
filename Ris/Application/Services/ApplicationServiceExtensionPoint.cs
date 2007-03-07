using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services
{
    [ExtensionPoint]
    public class ApplicationServiceExtensionPoint : ExtensionPoint<IApplicationServiceLayer>
    {
    }
}
