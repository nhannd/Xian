#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Rendering;
using DrawMode = ClearCanvas.ImageViewer.Rendering.DrawMode;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class MagnificationForm : Form
	{
        private readonly Point _startPointDesktop;
        private Point _startPointTile;
	    
        private IRenderingSurface _surface;

        private readonly RenderMagnifiedImage _render;

        public MagnificationForm(PresentationImage image, Point startPointTile, RenderMagnifiedImage render)
		{
			InitializeComponent();

            Visible = false;
			this.DoubleBuffered = false;
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

			if (Form.ActiveForm != null)
				this.Owner = Form.ActiveForm;

			_startPointTile = startPointTile;
            _render = render;

            _surface = image.ImageRenderer.GetRenderingSurface(Handle, ClientRectangle.Width, ClientRectangle.Height);

            _startPointDesktop = Centre = Cursor.Position;
		}

		#region Unused Code

		#endregion

		public void UpdateMousePosition(Point positionTile)
		{
			Size offsetFromStartTile = new Size(positionTile.X - _startPointTile.X, positionTile.Y - _startPointTile.Y);
			Point pointDesktop = _startPointDesktop;
			pointDesktop.Offset(offsetFromStartTile.Width, offsetFromStartTile.Height);
			Centre = pointDesktop;
		}

		private Point Centre
		{
			get
			{
				Point location = Location;
				location.Offset(Width / 2, Height / 2);
				return location;
			}	
			set
			{
				value.Offset(-Width / 2, -Height / 2);
				if (value != Location)
					base.Location = value;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            if (_surface != null)
            {
                _surface.ContextID = e.Graphics.GetHdc();
                var args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Refresh);
                _render(args);
                e.Graphics.ReleaseHdc(_surface.ContextID);
            }

		    //base.OnPaint(e);
        }

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged(e);
			RenderImage();
		}

		protected override void OnLocationChanged(System.EventArgs e)
		{
			base.OnLocationChanged(e);

			RenderImage();

			if (base.Visible && base.Owner != null)
				base.Owner.Update(); //update owner's invalidated region(s)
		}

        private void RenderImage()
        {
            if (_surface == null)
                return;

            using (var graphics = base.CreateGraphics())
            {
                _surface.ContextID = graphics.GetHdc();
                var args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Render);
                _render(args);
                args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Refresh);
                _render(args);
                graphics.ReleaseHdc(_surface.ContextID);
            }
        }
        
        private void DisposeSurface()
		{
			if (_surface != null)
			{
                _surface.Dispose();
                _surface = null;
			}
		}
	}
}