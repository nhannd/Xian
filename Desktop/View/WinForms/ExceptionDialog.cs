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
using ClearCanvas.Common;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(ExceptionDialogFactoryExtensionPoint))]
	public class ExceptionDialogFactory : IExceptionDialogFactory
	{
		#region IExceptionDialogFactory Members

		public IExceptionDialog CreateExceptionDialog()
		{
			return new ExceptionDialog();
		}

		#endregion
	}

	public class ExceptionDialog : Desktop.ExceptionDialog
	{
		private DialogBoxForm _form;

		protected override ExceptionDialogAction Show()
		{
			var control = new ExceptionDialogControl(Exception, Message, Actions, CloseForm, CloseForm);
			_form = new DialogBoxForm(Title, control, Size.Empty, DialogSizeHint.Auto);

			var screen = ScreenFromActiveForm() ?? ScreenFromMousePosition();
			int xdiff = screen.Bounds.Width - _form.Bounds.Width;
			int ydiff = screen.Bounds.Height - _form.Bounds.Height;
			int locationX = screen.WorkingArea.Left + Math.Max(0, (xdiff)/2);
			int locationY = screen.WorkingArea.Top + Math.Max(0, (ydiff)/2);

			_form.StartPosition = FormStartPosition.Manual;
			_form.Location = new Point(locationX, locationY);
			//_form.TopMost = true;
			_form.ShowDialog();

			return control.Result;
		}

		private void CloseForm()
		{
			_form.Close();
		}

		private static System.Windows.Forms.Screen ScreenFromActiveForm()
		{
			try
			{
				var activeForm = Form.ActiveForm;
				if (activeForm != null)
					return System.Windows.Forms.Screen.FromControl(activeForm);
			}
			catch
			{
			}

			return null;
		}

		private static System.Windows.Forms.Screen ScreenFromMousePosition()
		{
			return System.Windows.Forms.Screen.FromPoint(Control.MousePosition);
		}
	}
}
