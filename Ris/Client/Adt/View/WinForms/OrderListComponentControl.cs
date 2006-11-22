using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderListComponent"/>
    /// </summary>
    public partial class OrderListComponentControl : CustomUserControl
    {
        private OrderListComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderListComponentControl(OrderListComponent component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
            _orderList.Table = _component.Orders;
        }
    }
}
