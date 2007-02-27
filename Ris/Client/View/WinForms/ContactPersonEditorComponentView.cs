using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ContactPersonEditorComponentViewExtensionPoint))]
    public class ContactPersonEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ContactPersonEditorComponent _component;
        private ContactPersonEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ContactPersonEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ContactPersonEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
