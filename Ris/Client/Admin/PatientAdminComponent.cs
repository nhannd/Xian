using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientAdminComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public class PatientAdminComponent : ApplicationComponent
    {

    }
}
