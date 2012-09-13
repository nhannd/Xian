#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using System.Linq;
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.Tools;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	internal partial class ActionModelsToolComponentControl : UserControl
	{
		public ActionModelsToolComponentControl()
		{
			InitializeComponent();
		}

#if DEBUG

		private readonly ActionModelsToolComponent _component;

		public ActionModelsToolComponentControl(ActionModelsToolComponent component)
			: this()
		{
			_component = component;
			_component.ActiveComponentChanged += _component_ComponentChanged;

			_cboActionModel.Items.AddRange(component.AvailableActionModels.Cast<object>().ToArray());
		}

		private void _component_ComponentChanged(object sender, EventArgs e)
		{
			_pnlHostedControl.Controls.Clear();

			var control = _component.ActiveComponentHost.ComponentView.GuiElement as Control;
			if (control != null)
			{
				_pnlHostedControl.Controls.Add(control);
				control.Dock = DockStyle.Fill;
			}
		}

#endif

		private void button1_Click(object sender, EventArgs e)
		{
#if DEBUG
			_component.Accept();
#endif
		}

		private void button2_Click_1(object sender, EventArgs e)
		{
#if DEBUG
			_component.Cancel();
#endif
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
#if DEBUG
			_component.SelectedActionModel = _cboActionModel.SelectedItem as string;
#endif
		}
	}
}