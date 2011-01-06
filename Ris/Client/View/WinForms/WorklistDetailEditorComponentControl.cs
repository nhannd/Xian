#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="WorklistDetailEditorComponent"/>
	/// </summary>
	public partial class WorklistDetailEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly WorklistDetailEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public WorklistDetailEditorComponentControl(WorklistDetailEditorComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;
			_component.PropertyChanged += _component_PropertyChanged;

			_ownerGroupBox.Visible = _component.IsOwnerPanelVisible;
			_radioGroup.Enabled = _component.IsPersonalGroupSelectionEnabled;
			_radioGroup.DataBindings.Add("Checked", _component, "IsGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioPersonal.Enabled = _component.IsPersonalGroupSelectionEnabled;
			_radioPersonal.DataBindings.Add("Checked", _component, "IsPersonal", true, DataSourceUpdateMode.OnPropertyChanged);

			_groups.DataSource = _component.GroupChoices;
			_groups.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatGroup(args.ListItem); };
			_groups.DataBindings.Add("Enabled", _component, "IsGroupChoicesEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_groups.DataBindings.Add("Value", _component, "SelectedGroup", true, DataSourceUpdateMode.OnPropertyChanged);

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.Enabled = !_component.IsWorklistClassReadOnly;

			_worklistClass.DataSource = _component.WorklistClassChoices;
			_worklistClass.Enabled = !_component.IsWorklistClassReadOnly;
			_worklistClass.Format += delegate(object sender, ListControlConvertEventArgs args)
									 {
										 args.Value = _component.FormatWorklistClass(args.ListItem);
									 };

			_worklistClass.DataBindings.Add("Value", _component, "WorklistClass", true, DataSourceUpdateMode.OnPropertyChanged);

			_classDescription.DataBindings.Add("Value", _component, "WorklistClassDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_okButton.DataBindings.Add("Visible", _component, "AcceptButtonVisible", true, DataSourceUpdateMode.Never);
			_cancelButton.DataBindings.Add("Visible", _component, "CancelButtonVisible", true, DataSourceUpdateMode.Never);

		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "WorklistClassChoices")
			{
				_worklistClass.DataSource = _component.WorklistClassChoices;
			}
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
