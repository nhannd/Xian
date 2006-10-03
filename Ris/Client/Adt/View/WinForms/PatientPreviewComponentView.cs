using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientPreviewComponentViewExtensionPoint))]
    public class PatientPreviewComponentView : HtmlComponentView
    {
        public PatientPreviewComponentView()
            :base("PatientPreview.html")
        {

        }

        protected override ClearCanvas.Desktop.Actions.ActionModelNode GetEmbeddedActionModel()
        {
            return ((PatientPreviewComponent)this.Component).MenuModel;
        }
    }
}
