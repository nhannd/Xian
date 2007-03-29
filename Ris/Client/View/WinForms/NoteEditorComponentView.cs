using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="NoteEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(NoteEditorComponentViewExtensionPoint))]
    public class NoteEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private NoteEditorComponent _component;
        private NoteEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (NoteEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new NoteEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
