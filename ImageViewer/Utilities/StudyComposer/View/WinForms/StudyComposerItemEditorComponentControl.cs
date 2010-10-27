#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	public partial class StudyComposerItemEditorComponentControl : UserControl
	{
		private readonly StudyComposerItemEditorComponent _component;

		public StudyComposerItemEditorComponentControl()
		{
			InitializeComponent();
		}

		public StudyComposerItemEditorComponentControl(StudyComposerItemEditorComponent component) : this()
		{
			_component = component;
			Image icon = _component.Icon;
			if (icon == null)
			{
				picIcon.Visible = false;
			}
			else
			{
				this.IconSize = icon.Size;
				picIcon.Image = icon;
			}

			lblName.DataBindings.Add("Text", _component, "Name");
			lblDescription.DataBindings.Add("Text", _component, "Description");
			pgvProps.SelectedObject = _component.Node;
		}

		public Size IconSize
		{
			get { return picIcon.Size; }
			set
			{
				if (picIcon.Size != value)
				{
					pnlHeader.Size = new Size(pnlHeader.Width, value.Height + pnlHeader.Padding.Vertical);
					picIcon.Size = value;
				}
			}
		}

		private void btnOk_Click(object sender, EventArgs e) {
			_component.Ok();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			_component.Cancel();
		}

		private void btnApply_Click(object sender, EventArgs e) {
			_component.Apply();
		}
	}
}