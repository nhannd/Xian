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

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// WinForms implementation of <see cref="IWorkspaceDialogBoxView"/>. 
	/// </summary>
	public class WorkspaceDialogBoxView : DesktopObjectView, IWorkspaceDialogBoxView
	{
		private readonly WorkspaceView _owner;
		private WorkspaceDialogBoxForm _form;
		private Control _content;
		private bool _reallyClose;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="owner"></param>
		protected internal WorkspaceDialogBoxView(WorkspaceDialogBox dialogBox, WorkspaceView owner)
		{
			_owner = owner;

			var componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(dialogBox.Component.GetType());
			componentView.SetComponent((IApplicationComponent)dialogBox.Component);

			_content = (Control) componentView.GuiElement;
			_form = CreateDialogBoxForm(dialogBox, _content);
			_form.FormClosing += DialogBoxFormClosing;
		}

		/// <summary>
		/// Gets the dialog box form.
		/// </summary>
		internal WorkspaceDialogBoxForm DialogBoxForm
		{
			get { return _form; }
		}

		#region DesktopObjectView overrides

		/// <summary>
		/// Sets the title of the dialog box.
		/// </summary>
		/// <param name="title"></param>
		public override void SetTitle(string title)
		{
			_form.Text = title;
		}

		/// <summary>
		/// Opens the view.
		/// </summary>
		public override void Open()
		{
			_owner.AddDialogBoxView(this);
		}

		/// <summary>
		/// Not used.
		/// </summary>
		public override void Show()
		{
			// do nothing
		}

		/// <summary>
		/// Not used.
		/// </summary>
		public override void Hide()
		{
			// do nothing
		}

		/// <summary>
		/// Activates the view.
		/// </summary>
		public override void Activate()
		{
			_form.Activate();
		}

		/// <summary>
		/// Disposes of this object.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_form != null)
				{
					_reallyClose = true;

					// Remove the dialog box
					_owner.RemoveDialogBoxView(this);

					_content.Dispose();
					_content = null;
					_form.Dispose();
					_form = null;
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary>
		/// Called to create an instance of a <see cref="DialogBoxForm"/> for use by this view.
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		protected virtual WorkspaceDialogBoxForm CreateDialogBoxForm(WorkspaceDialogBox dialogBox, Control content)
		{
			return new WorkspaceDialogBoxForm(dialogBox, content);
		}

		private void DialogBoxFormClosing(object sender, FormClosingEventArgs e)
		{
			//When there is a "fatal exception", we terminate the gui toolkit, which calls Application.Exit().
			//So, we can't cancel the close, otherwise the application can get into a funny state.
			if (e.CloseReason == System.Windows.Forms.CloseReason.ApplicationExitCall || _reallyClose)
				return;
			
			e.Cancel = true;

			// raise the close requested event
			// if this results in an actual close, the Dispose method will be called, setting the _reallyClose flag
			RaiseCloseRequested();
		}
	}
}
