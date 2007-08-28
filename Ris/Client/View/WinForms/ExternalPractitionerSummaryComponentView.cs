using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ExternalPractitionerSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ExternalPractitionerSummaryComponentViewExtensionPoint))]
    public class ExternalPractitionerSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private ExternalPractitionerSummaryComponent _component;
        private ExternalPractitionerSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ExternalPractitionerSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ExternalPractitionerSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
