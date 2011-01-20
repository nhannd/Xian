#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerMergeAffectedOrdersComponent"/>.
	/// </summary>
	public partial class ExternalPractitionerMergeAffectedOrdersComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerMergeAffectedOrdersComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExternalPractitionerMergeAffectedOrdersComponentControl(ExternalPractitionerMergeAffectedOrdersComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_instruction.DataBindings.Add("Text", _component, "Instruction", true, DataSourceUpdateMode.OnPropertyChanged);

			_table.Items.AddRange(_component.AffectedOrderTableItems);
			_component.AllPropertiesChanged += OnAllPropertiesChanged;
		}

		private void OnAllPropertiesChanged(object sender, System.EventArgs e)
		{
			_table.Items.Clear();
			_table.Items.AddRange(_component.AffectedOrderTableItems);
		}
	}
}
