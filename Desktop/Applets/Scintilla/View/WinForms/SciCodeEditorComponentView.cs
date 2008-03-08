using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.Applets.Scintilla.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="SciCodeEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(SciCodeEditorComponentViewExtensionPoint))]
    public class SciCodeEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private SciCodeEditorComponent _component;
        private SciCodeEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (SciCodeEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new SciCodeEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
