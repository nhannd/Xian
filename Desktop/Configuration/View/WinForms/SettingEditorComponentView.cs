using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SettingEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SettingEditorComponentViewExtensionPoint))]
    public class SettingEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private SettingEditorComponent _component;
        private SettingEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SettingEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SettingEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
