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

		internal ImageViewerControl(ImageViewerComponent component)
		{
			_component = component;
			_physicalWorkspace = _component.PhysicalWorkspace as PhysicalWorkspace;

			InitializeComponent();

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.BackColor = Color.Black;

			_component.Closing += new EventHandler(OnComponentClosing);
			_physicalWorkspace.Drawing += new EventHandler(OnPhysicalWorkspaceDrawing);
			_physicalWorkspace.LayoutCompleted += new EventHandler(OnLayoutCompleted);
		}

		internal void Draw()
		{
			foreach (ImageBoxControl control in this.Controls)
				control.Draw();

			Invalidate();
		}

		#region Protected members

		protected override void OnLoad(EventArgs e)
		{
			AddImageBoxControls(_physicalWorkspace);

			base.OnLoad(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			this.SuspendLayout();

			foreach (ImageBoxControl control in this.Controls)
				control.ParentRectangle = this.ClientRectangle;

			this.ResumeLayout(false);

			Invalidate();
		}

		#endregion

		#region Private members

		private void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		void OnComponentClosing(object sender, EventArgs e)
		{
			foreach (Control control in this.Controls)
				control.Dispose();

			this.Controls.Clear();
		}

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			List<Control> oldControlList = new List<Control>();

			foreach (Control control in this.Controls)
				oldControlList.Add(control);

			// We add all the new tile controls to the image box control first,
			// then we remove the old ones. Removing them first then adding them
			// results in flickering, which we don't want.
			AddImageBoxControls(_physicalWorkspace);

			foreach (Control control in oldControlList)
			{
				this.Controls.Remove(control);
				control.Dispose();
			}
		}

		private void AddImageBoxControls(PhysicalWorkspace physicalWorkspace)
		{
			foreach (ImageBox imageBox in physicalWorkspace.ImageBoxes)
				AddImageBoxControl(imageBox);
		}

		private void AddImageBoxControl(ImageBox imageBox)
		{
			ImageBoxView view = ViewFactory.CreateAssociatedView(typeof(ImageBox)) as ImageBoxView;

			view.ImageBox = imageBox;
			view.ParentRectangle = this.ClientRectangle;
			
			ImageBoxControl control = view.GuiElement as ImageBoxControl;

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

		#endregion
	}
}
