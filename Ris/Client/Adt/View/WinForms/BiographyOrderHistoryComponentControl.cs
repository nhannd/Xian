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
    /// Provides a Windows Forms user-interface for <see cref="PatientOrderHistoryComponent"/>
    /// </summary>
    public partial class BiographyOrderHistoryComponentControl : CustomUserControl
    {
        private BiographyOrderHistoryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOrderHistoryComponentControl(BiographyOrderHistoryComponent component)
        {
            InitializeComponent();
            _component = component;

            _orderList.Table = _component.Orders;
        }
    }
}
