using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ExternalPractitionerMergeComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ExternalPractitionerMergeComponentViewExtensionPoint))]
    public class ExternalPractitionerMergeComponentView : WinFormsView, IApplicationComponentView
    {
        private ExternalPractitionerMergeComponent _component;
        private ExternalPractitionerMergeComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ExternalPractitionerMergeComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ExternalPractitionerMergeComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
