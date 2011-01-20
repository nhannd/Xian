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
	/// Provides a Windows Forms user-interface for <see cref="MergeOrdersComponent"/>.
	/// </summary>
	public partial class MergeOrdersComponentControl : ApplicationComponentUserControl
	{
		private readonly MergeOrdersComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MergeOrdersComponentControl(MergeOrdersComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_order1Description.Text = _component.Order1Description;
			_order2Description.Text = _component.Order2Description;
			UpdateMergeDirection();

			var previewControl = (Control)_component.MergedOrderPreviewComponentHost.ComponentView.GuiElement;
			_mergedOrderPreviewPanel.Controls.Add(previewControl);
			previewControl.Dock = DockStyle.Fill;
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _mergeDirectionButton_Click(object sender, EventArgs e)
		{
			_component.ToggleMergeDirection();
			UpdateMergeDirection();
		}

		private void UpdateMergeDirection()
		{
			_mergeDirectionButton.Image = _component.MergingRight ? SR.MergeRightSmall : SR.MergeLeftSmall;
		}
	}
}
