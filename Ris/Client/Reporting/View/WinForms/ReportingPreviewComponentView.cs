using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ReportingPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ReportingPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponentView : HtmlComponentView
    {
        public RegistrationPreviewComponentView()
            :base("ReportingPreview.html")
        {

        }

        protected override ClearCanvas.Desktop.Actions.ActionModelNode GetEmbeddedActionModel()
        {
            return ((ReportingPreviewComponent)this.Component).MenuModel;
        }
    }
}
