using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ImageViewerControl : UserControl
	{
		private PhysicalWorkspace _physicalWorkspace;
		private ImageViewerComponent _component;

		public ImageViewerControl(ImageViewerComponent component)
		{
			_component = component;

			InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.BackColor = Color.Black;

			_physicalWorkspace = _component.PhysicalWorkspace as PhysicalWorkspace;
			_physicalWorkspace.Drawing += new EventHandler(OnPhysicalWorkspaceDrawing);
			_physicalWorkspace.ImageBoxAdded += new EventHandler<ImageBoxEventArgs>(OnImageBoxAdded);
			_physicalWorkspace.ImageBoxRemoved += new EventHandler<ImageBoxEventArgs>(OnImageBoxRemoved);

			AddImageBoxControls(component);
		}

		public void Draw()
		{
			if (_physicalWorkspace.LayoutRefreshRequired)
			{
				LayoutImageBoxes();
			}
			else
			{
				foreach (ImageBoxControl control in this.Controls)
					control.Draw();
			}
		}

		void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			LayoutImageBoxes();

			base.OnSizeChanged(e);
		}

		void OnImageBoxAdded(object sender, ImageBoxEventArgs e)
		{
			AddImageBoxControl(e.ImageBox as ImageBox);
		}

		void OnImageBoxRemoved(object sender, ImageBoxEventArgs e)
		{
			foreach (ImageBoxControl control in this.Controls)
			{
				if (e.ImageBox == control.ImageBox)
				{
					control.Dispose();
					this.Controls.Remove(control);
					return;
				}
			}
		}

		private void OnContextMenuOpening(object sender, CancelEventArgs e)
		{
			ActionModelNode menuModel = _component.ContextMenuModel;

			if (menuModel != null)
			{
				ToolStripBuilder.Clear(_contextMenu.Items);
				ToolStripBuilder.BuildMenu(_contextMenu.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
		}

		private void LayoutImageBoxes()
		{
			this.SuspendLayout();

			foreach (ImageBoxControl control in this.Controls)
				control.ParentRectangle = this.ClientRectangle;

			this.ResumeLayout();
		}

		private void AddImageBoxControls(ImageViewerComponent component)
		{
			foreach (ImageBox imageBox in component.PhysicalWorkspace.ImageBoxes)
				AddImageBoxControl(imageBox);
		}

		private void AddImageBoxControl(ImageBox imageBox)
		{
			ImageBoxView view = ViewFactory.CreateAssociatedView(typeof(ImageBox)) as ImageBoxView;

			view.ImageBox = imageBox;

			ImageBoxControl control = view.GuiElement as ImageBoxControl;
			this.Controls.Add(control);
		}
	}
}
