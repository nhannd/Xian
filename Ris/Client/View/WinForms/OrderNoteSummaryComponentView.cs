using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="OrderNoteSummaryComponent"/>
    /// </summary>
    [ExtensionOf(typeof(OrderNoteSummaryComponentViewExtensionPoint))]
    public class OrderNoteSummaryComponentView : WinFormsView, IApplicationComponentView
    {
        private OrderNoteSummaryComponent _component;
        private OrderNoteSummaryComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (OrderNoteSummaryComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new OrderNoteSummaryComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
