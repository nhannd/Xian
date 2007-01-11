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
    /// Provides a Windows Forms user-interface for <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    public partial class ContactPersonEditorComponentControl : ApplicationComponentUserControl
    {
        private ContactPersonEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponentControl(ContactPersonEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);

            _relationship.DataSource = _component.RelationshipChoices;
            _relationship.DataBindings.Add("Value", _component, "Relationship", true, DataSourceUpdateMode.OnPropertyChanged);

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _address.DataBindings.Add("Value", _component, "Address", true, DataSourceUpdateMode.OnPropertyChanged);
            _homePhone.DataBindings.Add("Value", _component, "HomePhoneNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _businessPhone.DataBindings.Add("Value", _component, "BusinessPhoneNumber", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
