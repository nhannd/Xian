using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Admin
{
    public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumbersEditorComponent : ApplicationComponent
    {

    }
}
