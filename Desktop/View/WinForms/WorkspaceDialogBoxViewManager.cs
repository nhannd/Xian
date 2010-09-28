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
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Manages the set of workspace dialog views for a <see cref="WorkspaceView"/>.
	/// </summary>
	internal class WorkspaceDialogBoxViewManager : IDisposable
	{
		private readonly WorkspaceView _owner;
		private readonly Control _workspaceContent;

		private readonly List<WorkspaceDialogBoxForm> _dialogBoxForms = new List<WorkspaceDialogBoxForm>();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="workspaceContent"></param>
		internal WorkspaceDialogBoxViewManager(WorkspaceView owner, Control workspaceContent)
		{
			_owner = owner;
			_workspaceContent = workspaceContent;

			// subscribe to some events so we can keep workspace dialogs appropriately sized/visible
			_workspaceContent.VisibleChanged += WorkspaceContentVisibleChanged;
			_workspaceContent.SizeChanged += WorkspaceContentSizeChanged;
			_owner.DesktopView.DesktopForm.LocationChanged += DesktopFormLocationChanged;
		}

		public void Dispose()
		{
			// unsubscribe from events so we don't have memory leaks
			_owner.DesktopView.DesktopForm.LocationChanged -= DesktopFormLocationChanged;
			_workspaceContent.VisibleChanged -= WorkspaceContentVisibleChanged;
			_workspaceContent.SizeChanged -= WorkspaceContentSizeChanged;
		}

		/// <summary>
		/// Adds the specified dialog box view.
		/// </summary>
		/// <param name="view"></param>
		public void AddDialogBoxView(WorkspaceDialogBoxView view)
		{
			// ensure content and any existing dialogs are disabled
			_workspaceContent.Enabled = false;
			CollectionUtils.ForEach(_dialogBoxForms, lb => lb.Enabled = false);

			// position and display the new dialog
			var dialogForm = view.DialogBoxForm;
			dialogForm.CentreInControl(_workspaceContent);

			// show, with the owner as the desktop form
			dialogForm.Show(_owner.DesktopView.DesktopForm);

			// add to list
			_dialogBoxForms.Add(dialogForm);
		}

		/// <summary>
		/// Removes the specified dialog box view.
		/// </summary>
		/// <param name="view"></param>
		public void RemoveDialogBoxView(WorkspaceDialogBoxView view)
		{
			// close the dialog and remove it
			var form = view.DialogBoxForm;
			form.Close();
			_dialogBoxForms.Remove(form);

			// re-enable the appropriate thing
			var previousDialogBox = CollectionUtils.LastElement(_dialogBoxForms);
			if (previousDialogBox != null)
			{
				// enable and activate previous dialog box in the list
				previousDialogBox.Enabled = true;
				previousDialogBox.Activate();
			}
			else
			{
				// activate the workspace content
				_workspaceContent.Enabled = true;
				_owner.Activate();
			}
		}

		#region Event Handlers

		private void DesktopFormLocationChanged(object sender, EventArgs e)
		{
			if (!_workspaceContent.Visible)
				return;

			UpdateDialogPositions();
		}

		private void WorkspaceContentSizeChanged(object sender, EventArgs e)
		{
			if (!_workspaceContent.Visible)
				return;

			UpdateDialogPositions();
		}

		private void WorkspaceContentVisibleChanged(object sender, EventArgs e)
		{
			// show or hide dialogs in response to workspace activation changing
			foreach (var form in _dialogBoxForms)
			{
				form.Visible = _workspaceContent.Visible;
			}

			// if the control has just become visible, it is possible that
			// the dialogs are not properly positioned, so we need to update them
			if (_workspaceContent.Visible)
			{
				UpdateDialogPositions();
			}
		}

		#endregion

		/// <summary>
		/// Resets the position of all dialog box forms.
		/// </summary>
		private void UpdateDialogPositions()
		{
			foreach (var form in _dialogBoxForms)
			{
				form.CentreInControl(_workspaceContent);
			}
		}
	}
}
