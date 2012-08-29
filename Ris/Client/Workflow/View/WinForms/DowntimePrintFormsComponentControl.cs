#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="DowntimePrintFormsComponent"/>
	/// </summary>
	public partial class DowntimePrintFormsComponentControl : ApplicationComponentUserControl
	{
		private readonly DowntimePrintFormsComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public DowntimePrintFormsComponentControl(DowntimePrintFormsComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_numberOfForms.DataBindings.Add("Value", _component, "NumberOfFormsToPrint", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			_component.Print();
		}

		private void _cancelPrintingButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
	}
}
