using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="EmailAddressSummaryComponent"/>
    /// </summary>
    public partial class EmailAddressesSummaryComponentControl : ApplicationComponentUserControl
    {
        private EmailAddressesSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailAddressesSummaryComponentControl(EmailAddressesSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _emailAddressList.ToolbarModel = _component.EmailAddressListActionModel;
            _emailAddressList.MenuModel = _component.EmailAddressListActionModel;

            _emailAddressList.Table = _component.EmailAddresses;
            _emailAddressList.DataBindings.Add("Selection", _component, "SelectedEmailAddress", true, DataSourceUpdateMode.OnPropertyChanged);

            // move the error icon up to the top of the table, rather than the middle
            this.ErrorProvider.SetIconAlignment(_emailAddressList, ErrorIconAlignment.TopRight);
        }
    }
}
