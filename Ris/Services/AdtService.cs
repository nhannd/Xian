using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AdtService : HealthcareServiceLayer, IAdtService
    {
    }
}
