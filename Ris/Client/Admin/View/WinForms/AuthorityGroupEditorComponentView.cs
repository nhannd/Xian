using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="AuthorityGroupEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(AuthorityGroupEditorComponentViewExtensionPoint))]
    public class AuthorityGroupEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private AuthorityGroupEditorComponent _component;
        private AuthorityGroupEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AuthorityGroupEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AuthorityGroupEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
