using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(ButtonActionViewExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
    public class ButtonActionView : IActionView
    {
        private IClickAction _action;
        private ActiveToolbarButton _button;

        public ButtonActionView()
        {
        }

        #region IActionView Members

        public void SetAction(IAction action)
        {
            _action = (IClickAction)action;
        }

        #endregion

        #region IView Members

        public GuiToolkitID GuiToolkitID
        {
            get { return GuiToolkitID.WinForms; }
        }

        public object GuiElement
        {
            get
            {
                if (_button == null)
                {
                    _button = new ActiveToolbarButton(_action);
                }
                return _button;
            }
        }

        #endregion
    }
}
