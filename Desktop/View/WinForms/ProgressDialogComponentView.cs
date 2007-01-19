using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ProgressDialogComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ProgressDialogComponentViewExtensionPoint))]
    public class ProgressDialogComponentView : WinFormsView, IApplicationComponentView
    {
        private ProgressDialogComponent _component;
        private ProgressDialogComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ProgressDialogComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ProgressDialogComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
