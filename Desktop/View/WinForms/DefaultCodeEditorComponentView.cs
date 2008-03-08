using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DefaultCodeEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DefaultCodeEditorComponentViewExtensionPoint))]
    public class DefaultCodeEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private DefaultCodeEditorComponent _component;
        private DefaultCodeEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DefaultCodeEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DefaultCodeEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
