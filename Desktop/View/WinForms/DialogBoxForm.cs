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
using ClearCanvas.Common;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Form used by the <see cref="DialogBoxView"/> class.
	/// </summary>
	/// <remarks>
	/// This class may be subclassed.
	/// </remarks>
	public partial class DialogBoxForm : DotNetMagicForm
	{
		private readonly Control _content;
		private DialogBoxAction _closeAction;

		internal DialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint)
			: this(title, content, exactSize, sizeHint, false) {}

		internal DialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint, bool allowResize)
		{
			InitializeComponent();
			Text = title;

			_content = content;

			// important - if we do not set a minimum size, the full content may not be displayed
			_content.MinimumSize = _content.Size;
			_content.Dock = DockStyle.Fill;

			// adjust size of client area
			this.ClientSize = exactSize != Size.Empty ? exactSize : SizeHintHelper.TranslateHint(sizeHint, _content.Size);

			if (allowResize)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				MinimumSize = base.SizeFromClientSize(_content.Size);
			}

			_contentPanel.Controls.Add(_content);

			// Resize the dialog if size of the underlying content changed
			_content.SizeChanged += OnContentSizeChanged;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="content"></param>
		public DialogBoxForm(DialogBox dialogBox, Control content)
			: this(dialogBox.Title, content, dialogBox.Size, dialogBox.DialogSizeHint, dialogBox.AllowUserResize)
		{
		}

		internal void DelayedClose(DialogBoxAction action)
		{
			_closeAction = action;
			_delayedCloseTimer.Enabled = true;
		}

		private void OnContentSizeChanged(object sender, EventArgs e)
		{
			if (ClientSize != _content.Size)
				ClientSize = _content.Size;
		}

		private void _delayedCloseTimer_Tick(object sender, EventArgs e)
		{
			// disable timer so it doesn't fire again
			_delayedCloseTimer.Enabled = false;

			// close the form
			switch (_closeAction)
			{
				case DialogBoxAction.Cancel:
					DialogResult = DialogResult.Cancel;
					break;
				case DialogBoxAction.Ok:
					DialogResult = DialogResult.OK;
					break;
			}
		}
	}
}