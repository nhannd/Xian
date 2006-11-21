using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="OrderEntryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(OrderEntryComponentViewExtensionPoint))]
    public class OrderEntryComponentView : WinFormsView, IApplicationComponentView
    {
        private OrderEntryComponent _component;
        private OrderEntryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (OrderEntryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new OrderEntryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
