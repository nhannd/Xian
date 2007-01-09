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
    [ExtensionOf(typeof(StaffDetailsEditorComponentViewExtensionPoint))]
    public class PractitionerDetailssEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private StaffDetailsEditorComponent _component;
        private StaffDetailsEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (StaffDetailsEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new StaffDetailsEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
