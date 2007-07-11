using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TechnologistPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TechnologistPreviewComponentViewExtensionPoint))]
    public class TechnologistPreviewComponentView : PreviewComponentView
    {
    }
}
