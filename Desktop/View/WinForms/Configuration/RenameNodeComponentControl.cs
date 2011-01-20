#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class RenameNodeComponentControl : ApplicationComponentUserControl
	{
		public RenameNodeComponentControl(RenameNodeComponent component) : base(component)
		{
			InitializeComponent();

			_txtName.DataBindings.Add("Value", component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}