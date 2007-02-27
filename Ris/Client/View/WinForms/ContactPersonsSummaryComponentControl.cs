using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ContactPersonsSummaryComponent"/>
    /// </summary>
    public partial class ContactPersonsSummaryComponentControl : ApplicationComponentUserControl
    {
        private ContactPersonsSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonsSummaryComponentControl(ContactPersonsSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _contactPersonList.ToolbarModel = _component.ContactPersonListActionModel;
            _contactPersonList.MenuModel = _component.ContactPersonListActionModel;

            _contactPersonList.Table = _component.ContactPersons;
            _contactPersonList.DataBindings.Add("Selection", _component, "SelectedContactPerson", true, DataSourceUpdateMode.OnPropertyChanged);

            // move the error icon up to the top of the table, rather than the middle
            this.ErrorProvider.SetIconAlignment(_contactPersonList, ErrorIconAlignment.TopRight);
        }
    }
}
