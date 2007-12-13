using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="OrderNoteEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(OrderNoteEditorComponentViewExtensionPoint))]
    public class OrderNoteEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private OrderNoteEditorComponent _component;
        private OrderNoteEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (OrderNoteEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new OrderNoteEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
