using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="WorklistEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(WorklistEditorComponentViewExtensionPoint))]
    public class WorklistEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private WorklistEditorComponent _component;
        private WorklistEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (WorklistEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new WorklistEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
