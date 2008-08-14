using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CannedTextEditorComponent"/>
    /// </summary>
    public partial class CannedTextEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly CannedTextEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CannedTextEditorComponentControl(CannedTextEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_typeGroupBox.Visible = _component.CanChangeType;
			if (_component.CanChangeType == false && (_component.HasGroupAdminAuthority == false || _component.HasPersonalAdminAuthority == false))
				_groups.Visible = false;
			else
				_radioGroup.Checked = _component.IsEditingGroup;

			_radioPersonal.DataBindings.Add("Checked", _component, "IsEditingPersonal", true, DataSourceUpdateMode.OnPropertyChanged);

			_groups.DataSource = _component.StaffGroupChoices;
			_groups.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatStaffGroup(args.ListItem); };
			_groups.DataBindings.Add("Value", _component, "StaffGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_groups.DataBindings.Add("Enabled", _component, "IsEditingGroup", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.DataBindings.Add("Text", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_text.DataBindings.Add("Value", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
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
