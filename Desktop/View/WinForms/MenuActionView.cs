using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [ExtensionOf(typeof(MenuActionViewExtensionPoint))]
    [GuiToolkit(GuiToolkitID.WinForms)]
    public class MenuActionView : IActionView
    {
        private IClickAction _action;
        private ActiveMenuItem _menuItem;

        public MenuActionView()
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
                if (_menuItem == null)
                {
                    _menuItem = new ActiveMenuItem(_action);
                }
                return _menuItem;
            }
        }

        #endregion
    }
}
