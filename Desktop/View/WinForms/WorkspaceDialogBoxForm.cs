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
using System.Drawing;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Form that implements the workspace dialog.
	/// </summary>
	public partial class WorkspaceDialogBoxForm : Form
	{
		private readonly Control _content;
		private readonly Size _idealSize;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="content"></param>
		internal WorkspaceDialogBoxForm(WorkspaceDialogBox dialogBox, Control content)
			: this(dialogBox.Title, content, dialogBox.Size, dialogBox.SizeHint)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="content"></param>
		/// <param name="exactSize"></param>
		/// <param name="sizeHint"></param>
		private WorkspaceDialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint)
		{
			InitializeComponent();
			this.Text = title;

			_content = content;

			// important - if we do not set a minimum size, the full content may not be displayed
			_content.MinimumSize = _content.Size;
			_content.Dock = DockStyle.Fill;

			// adjust size of client area to its ideal size
			var contentSize = exactSize != Size.Empty ? exactSize : SizeHintHelper.TranslateHint(sizeHint, _content.Size);
			this.ClientSize = contentSize;// +new Size(0, 4);

			// record the ideal size for future reference
			_idealSize = this.Size;

			_contentPanel.Controls.Add(_content);
		}

		/// <summary>
		/// Position this form in the centre of the specified control.
		/// </summary>
		internal void CentreInControl(Control control)
		{
			// max size of this form is the size of the specified control
			var maxSize = control.Size;

			// computer centre of host control in screen coordinates
			var centre = control.PointToScreen(new Point(0, 0));
			centre.Offset(control.Width / 2, control.Height / 2);

			// compute size of form
			var w = Math.Min(_idealSize.Width, maxSize.Width);
			var h = Math.Min(_idealSize.Height, maxSize.Height);

			// compute upper left corner location
			var x = centre.X - (w/2);
			var y = centre.Y - (h/2);
			
			// update position
			this.Bounds = new Rectangle(x, y, w, h);
		}
	}
}
