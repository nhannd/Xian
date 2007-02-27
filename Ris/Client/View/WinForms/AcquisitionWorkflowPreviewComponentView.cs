using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="AcquisitionWorkflowPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(AcquisitionWorkflowPreviewComponentViewExtensionPoint))]
    public class AcquisitionWorkflowPreviewComponentView : HtmlComponentView
    {
        public AcquisitionWorkflowPreviewComponentView()
            : base("AcquisitionWorkflowPreview.html")
        {

        }
    }
}
