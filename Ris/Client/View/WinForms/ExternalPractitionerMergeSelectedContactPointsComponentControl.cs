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
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerMergeSelectedContactPointsComponent"/>.
	/// </summary>
	public partial class ExternalPractitionerMergeSelectedContactPointsComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerMergeSelectedContactPointsComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExternalPractitionerMergeSelectedContactPointsComponentControl(ExternalPractitionerMergeSelectedContactPointsComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_instruction.DataBindings.Add("Text", _component, "Instruction", true, DataSourceUpdateMode.OnPropertyChanged);

			_table.Table = _component.ContactPointTable;
			_table.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
