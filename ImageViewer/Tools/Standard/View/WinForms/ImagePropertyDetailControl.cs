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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using Application=ClearCanvas.Desktop.Application;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class ImagePropertyDetailControl : UserControl
	{
		public ImagePropertyDetailControl(string name, string description, string value)
		{
			InitializeComponent();

			_richText.Text = value;
			_name.Text = name;
			_description.Text = description;
		}
	}

	internal class DummyComponent : ApplicationComponent
	{
		public DummyComponent()
		{
		}
	}

	internal class CancelController : IButtonControl
	{
		readonly Form _parent;

		public CancelController(Form parent)
		{
			_parent = parent;
		}

		#region IButtonControl Members

		public DialogResult DialogResult
		{
			get
			{
				return System.Windows.Forms.DialogResult.Cancel;
			}
			set
			{
			}
		}

		public void NotifyDefault(bool value)
		{
		}

		public void PerformClick()
		{
			_parent.Close();
		}

		#endregion
	}

	internal class ShowValueDialog : DialogBox
	{
		private ShowValueDialog(string text)
			: base(CreateArgs(), Application.ActiveDesktopWindow)
        {
        }

		public static void Show(string name, string description, string text)
		{
			ShowValueDialog dialog = new ShowValueDialog(text);
			DialogBoxForm form = new DialogBoxForm(dialog, new ImagePropertyDetailControl(name, description, text));
			form.Text = SR.TitleDetails;
			form.CancelButton = new CancelController(form);
			form.StartPosition = FormStartPosition.Manual;
			form.DesktopLocation = Cursor.Position - new Size(form.DesktopBounds.Width/2, form.DesktopBounds.Height/2);
			form.ShowDialog();
			form.Dispose();
		}

		private static DialogBoxCreationArgs CreateArgs()
		{
			DialogBoxCreationArgs args = new DialogBoxCreationArgs();
			args.Component = new DummyComponent();
			args.SizeHint = DialogSizeHint.Auto;
			return args;
		}
	}
}
