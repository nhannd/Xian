using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PatientProfileDiffComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PatientProfileDiffComponentViewExtensionPoint))]
    public class PatientProfileDiffComponentView : HtmlComponentView
    {
        public PatientProfileDiffComponentView()
            :base("PatientProfileDiff.html")
        {
        }
    }
}
