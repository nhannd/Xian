#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
