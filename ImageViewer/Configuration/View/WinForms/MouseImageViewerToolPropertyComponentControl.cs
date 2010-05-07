#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Left);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Right);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.Middle);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton1);
			_cboActiveMouseButtons.Items.Add(XMouseButtons.XButton2);
			_cboActiveMouseButtons.SelectedIndex = 0;
			_cboActiveMouseButtons.DataBindings.Add("SelectedItem", component, "ActiveMouseButtons", true, DataSourceUpdateMode.OnPropertyChanged);

			_cboGlobalMouseButtons.Format += OnCboActiveMouseButtonsFormat;
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

		private static void OnCboActiveMouseButtonsFormat(object sender, ListControlConvertEventArgs e)
		{
			if (e.DesiredType == typeof (string))
			{
				e.Value = TypeDescriptor.GetConverter(typeof (XMouseButtons)).ConvertToString(e.ListItem);
			}
		}

		private static void OnKeyModifierBindingConvert(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType == typeof (ModifierFlags))
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
			else if (e.DesiredType == typeof (XKeys))
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