using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="TechnologistPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(TechnologistPreviewComponentViewExtensionPoint))]
    public class TechnologistPreviewComponentView : HtmlComponentView
    {
        public TechnologistPreviewComponentView()
            :base("TechnologistPreview.html")
	    {
	    }

        protected override ClearCanvas.Desktop.Actions.ActionModelNode GetEmbeddedActionModel()
        {
            return ((TechnologistPreviewComponent)this.Component).MenuModel;
        }

    }
}
