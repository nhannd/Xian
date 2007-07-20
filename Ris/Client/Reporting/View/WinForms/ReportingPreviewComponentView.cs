using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Ris.Client.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ReportingPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ReportingPreviewComponentViewExtensionPoint))]
    public class ReportingPreviewComponentView : DHtmlComponentView
    {
    }
}
