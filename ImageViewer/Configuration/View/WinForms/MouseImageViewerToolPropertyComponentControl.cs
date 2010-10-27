#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class MouseImageViewerToolPropertyComponentControl : UserControl
	{
		public MouseImageViewerToolPropertyComponentControl(MouseImageViewerToolPropertyComponent component)
		{
			InitializeComponent();

			_chkInitiallySelected.DataBindings.Add("Checked", component, "InitiallyActive", false, DataSourceUpdateMode.OnPropertyChanged);

			_cboActiveMouseButtons.Format += OnCboActiveMouseButtonsFormat;
			_cboActiveMouseButtons.SelectedIndexChanged += OnComboBoxSelectedItemChangedUpdate;
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Left);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Right);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Middle);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton1);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton2);
			_cboActiveMouseButtons.SelectedIndex = 0;
			_cboActiveMouseButtons.DataBindings.Add("SelectedItem", component, "ActiveMouseButtons", true, DataSourceUpdateMode.OnPropertyChanged);

			_cboGlobalMouseButtons.Format += OnCboActiveMouseButtonsFormat;
			_cboGlobalMouseButtons.SelectedIndexChanged += OnComboBoxSelectedItemChangedUpdate;
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.None);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Left);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Right);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.Middle);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.XButton1);
			_cboGlobalMouseButtons.Items.Add(XMouseButtons.XButton2);
			_cboGlobalMouseButtons.SelectedIndex = 0;
			_cboGlobalMouseButtons.DataBindings.Add("SelectedItem", component, "GlobalMouseButtons", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding keyModifierBinding = new Binding("KeyModifiers", component, "GlobalModifiers", true, DataSourceUpdateMode.OnPropertyChanged);
			keyModifierBinding.Format += OnKeyModifierBindingConvert;
			keyModifierBinding.Parse += OnKeyModifierBindingConvert;
			_chkGlobalModifiers.DataBindings.Add(keyModifierBinding);
		}

		private static void OnComboBoxSelectedItemChangedUpdate(object sender, EventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				// someone needs to explain why data binding SelectedItem on property change doesn't work out of the box
				var binding = comboBox.DataBindings["SelectedItem"];
				if (binding != null && binding.DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged)
					binding.WriteValue();
			}
		}

		private static void OnCboActiveMouseButtonsFormat(object sender, ListControlConvertEventArgs e)
		{
			if (e.DesiredType == typeof (string))
			{
				e.Value = TypeDescriptor.GetConverter(typeof (XMouseButtons)).ConvertToString(e.ListItem);
			}
		}

		private static void OnKeyModifierBindingConvert(object sender, ConvertEventArgs e)
		{
			if (e.Value is XKeys && e.DesiredType == typeof (ModifierFlags))
			{
				ModifierFlags result = ModifierFlags.None;
				XKeys value = (XKeys) e.Value;
				if ((value & XKeys.Control) == XKeys.Control)
					result = result | ModifierFlags.Control;
				if ((value & XKeys.Alt) == XKeys.Alt)
					result = result | ModifierFlags.Alt;
				if ((value & XKeys.Shift) == XKeys.Shift)
					result = result | ModifierFlags.Shift;
				e.Value = result;
			}
			else if (e.Value is ModifierFlags && e.DesiredType == typeof (XKeys))
			{
				XKeys result = XKeys.None;
				ModifierFlags value = (ModifierFlags) e.Value;
				if ((value & ModifierFlags.Control) == ModifierFlags.Control)
					result = result | XKeys.Control;
				if ((value & ModifierFlags.Alt) == ModifierFlags.Alt)
					result = result | XKeys.Alt;
				if ((value & ModifierFlags.Shift) == ModifierFlags.Shift)
					result = result | XKeys.Shift;
				e.Value = result;
			}
		}

		private void OnCboGlobalMouseButtonsSelectedIndexChanged(object sender, EventArgs e)
		{
			_chkGlobalModifiers.Enabled = (XMouseButtons) _cboGlobalMouseButtons.SelectedItem != XMouseButtons.None;
		}
	}
}