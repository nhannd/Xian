using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ExceptionHandlerComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ExceptionHandlerComponentViewExtensionPoint))]
    public class ExceptionHandlerComponentView : WinFormsView, IApplicationComponentView
    {
        private ExceptionHandlerComponent _component;
        private ExceptionHandlerComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ExceptionHandlerComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ExceptionHandlerComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
