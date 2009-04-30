#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	public partial class ImageViewerControl : UserControl
	{
		private Form _parentForm;
		private PhysicalWorkspace _physicalWorkspace;
		private ImageViewerComponent _component;
		private DelayedEventPublisher _delayedEventPublisher;

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
			_physicalWorkspace.ScreenRectangleChanged += new EventHandler(OnScreenRectangleChanged);

			_delayedEventPublisher = new DelayedEventPublisher(OnRecalculateImageBoxes, 50);
		}

		internal void Draw()
		{
			foreach (ImageBoxControl control in this.Controls)
				control.Draw();

			Invalidate();
		}

		#region Protected members

		protected override void OnParentChanged(EventArgs e) {
			SetParentForm(base.ParentForm);
	
			base.OnParentChanged(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			AddImageBoxControls(_physicalWorkspace);

			base.OnLoad(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			UpdateScreenRectangle();
		}

		#endregion

		#region Private members

		private void OnParentMoved(object sender, EventArgs e)
		{
			UpdateScreenRectangle();
		}

		private void OnScreenRectangleChanged(object sender, EventArgs e)
		{
			_delayedEventPublisher.Publish(this, EventArgs.Empty);
		}

		private void OnRecalculateImageBoxes(object sender, EventArgs e)
		{
			this.SuspendLayout();

			foreach (ImageBoxControl control in this.Controls)
				control.ParentRectangle = this.ClientRectangle;

			this.ResumeLayout(false);

			Invalidate();
		}

		private void OnPhysicalWorkspaceDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		void OnComponentClosing(object sender, EventArgs e)
		{
			while (this.Controls.Count > 0)
				this.Controls[0].Dispose();
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

		private void UpdateScreenRectangle()
		{
			_physicalWorkspace.ScreenRectangle = this.RectangleToScreen(this.ClientRectangle);
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

		private void SetParentForm(Form value) {
			if (_parentForm != value) {
				if (_parentForm != null)
					_parentForm.Move -= OnParentMoved;

				_parentForm = value;

				if (_parentForm != null)
					_parentForm.Move += OnParentMoved;
			}
		}

		#endregion
	}
}
