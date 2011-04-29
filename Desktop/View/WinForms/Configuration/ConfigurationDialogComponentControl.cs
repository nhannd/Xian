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
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ConfigurationDialogComponentControl : ApplicationComponentUserControl
	{
		private ConfigurationDialogComponent _component;

		internal ConfigurationDialogComponentControl(ConfigurationDialogComponent component, Control navigatorControl)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			component.PropertyChanged += OnPropertyChanged;

			_warningMessage.DataBindings.Add("Text", _component, "ConfigurationWarning", true);

			UpdateOfflineWarningVisibility();
			_tableLayoutPanel.Controls.Add(navigatorControl, 0, 1);

			Size = _tableLayoutPanel.Size;
			_tableLayoutPanel.SizeChanged += (s, e) => Size = _tableLayoutPanel.Size;
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ConfigurationWarning")
				UpdateOfflineWarningVisibility();
		}

		private void UpdateOfflineWarningVisibility()
		{
			var rowStyle = _tableLayoutPanel.RowStyles[0];
			if (String.IsNullOrEmpty(_component.ConfigurationWarning))
			{
				_warningLayoutTable.Visible = false;
				rowStyle.SizeType = SizeType.Absolute;
				rowStyle.Height = 0;
			}
			else
			{
				_warningLayoutTable.Visible = true;
				rowStyle.SizeType = SizeType.AutoSize;
			}
		}
	}
}
