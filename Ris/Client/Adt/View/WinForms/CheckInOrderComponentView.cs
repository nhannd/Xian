using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="CheckInOrderComponent"/>
    /// </summary>
    [ExtensionOf(typeof(CheckInOrderComponentViewExtensionPoint))]
    public class CheckInOrderComponentView : WinFormsView, IApplicationComponentView
    {
        private CheckInOrderComponent _component;
        private CheckInOrderComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (CheckInOrderComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new CheckInOrderComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
