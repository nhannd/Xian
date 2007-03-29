using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="NoteCategoryEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(NoteCategoryEditorComponentViewExtensionPoint))]
    public class NoteCategoryEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private NoteCategoryEditorComponent _component;
        private NoteCategoryEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (NoteCategoryEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new NoteCategoryEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
