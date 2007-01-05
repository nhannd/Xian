using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PractitionerSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PractitionerSummaryComponentViewExtensionPoint))]
    public class PractitionerSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private PractitionerSummaryComponent _component;
        private PractitionerSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PractitionerSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PractitionerSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
