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
    [ExtensionOf(typeof(RegistrationPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponentView : HtmlComponentView
    {
        public RegistrationPreviewComponentView()
            :base("RegistrationPreview.html")
        {

        }

        protected override ClearCanvas.Desktop.Actions.ActionModelNode GetEmbeddedActionModel()
        {
            return ((RegistrationPreviewComponent)this.Component).MenuModel;
        }
    }
}
