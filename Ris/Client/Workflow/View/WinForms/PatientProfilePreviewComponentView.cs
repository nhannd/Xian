#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientPreviewComponentViewExtensionPoint))]
    public class PatientProfilePreviewComponentView : HtmlComponentView
    {
        public PatientProfilePreviewComponentView()
            :base("PatientPreview.html")
        {

        }

        protected override ClearCanvas.Desktop.Actions.ActionModelNode GetEmbeddedActionModel()
        {
            return ((PatientProfilePreviewComponent)this.Component).MenuModel;
        }
    }
}
