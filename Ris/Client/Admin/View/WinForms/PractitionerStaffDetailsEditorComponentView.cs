using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="PractitionerDetailsEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(PractitionerStaffDetailsEditorComponentViewExtensionPoint))]
    public class PractitionerDetailssEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PractitionerStaffDetailsEditorComponent _component;
        private PractitionerStaffDetailsEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PractitionerStaffDetailsEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PractitionerStaffDetailsEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
