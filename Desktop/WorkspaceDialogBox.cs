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


using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Represents a workspace-modal dialog box.
	/// </summary>
	public class WorkspaceDialogBox : DesktopObject, IWorkspaceDialogBox
	{
		#region Host Implementation

		// implements the host interface, which is exposed to the hosted application component
		private class Host : ApplicationComponentHost, IWorkspaceDialogBoxHost
		{
			private readonly WorkspaceDialogBox _owner;

			internal Host(WorkspaceDialogBox owner, IApplicationComponent component)
				: base(component)
			{
				Platform.CheckForNullReference(owner, "owner");
				_owner = owner;
			}

			public override void Exit()
			{
				_owner._exitRequestedByComponent = true;

				// close the dialog
				_owner.Close(UserInteraction.Allowed, CloseReason.Program);
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _owner.Workspace.DesktopWindow; }
			}

			public override string Title
			{
				get { return _owner.Title; }
				set { _owner.Title = value; }
			}
		}

		#endregion

		private Host _host;
		private bool _exitRequestedByComponent;
		private readonly DialogSizeHint _sizeHint;
		private readonly Size _size;
		private Workspace _workspace;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="workspace"></param>
		protected internal WorkspaceDialogBox(DialogBoxCreationArgs args, Workspace workspace)
			: base(args)
		{
			_workspace = workspace;
			_host = new Host(this, args.Component);
			_size = args.Size;
			_sizeHint = args.SizeHint;
		}

		#region Public properties

		/// <summary>
		/// Gets the hosted component.
		/// </summary>
		public object Component
		{
			get { return _host.Component; }
		}

		/// <summary>
		/// Gets the workspace that owns this dialog box.
		/// </summary>
		public Workspace Workspace
		{
			get { return _workspace; }
		}

		/// <summary>
		/// Gets the size hint.
		/// </summary>
		public DialogSizeHint SizeHint
		{
			get { return _sizeHint; }
		}

		/// <summary>
		/// Gets the explicit size of the dialog, if specified.
		/// </summary>
		public Size Size
		{
			get { return _size; }
		}

		#endregion

		#region Protected overrides

		/// <summary>
		/// Asks the object whether it is in a closable state without user intervention.
		/// </summary>
		/// <returns>True if the object can be closed, otherwise false.</returns>
		protected internal override bool CanClose()
		{
			return _exitRequestedByComponent || _host.Component.CanExit();
		}

		/// <summary>
		/// Gives the hosted component a chance to prepare for a forced exit.
		/// </summary>
		protected override bool PrepareClose(CloseReason reason)
		{
			base.PrepareClose(reason);

			return _exitRequestedByComponent || _host.Component.PrepareExit();
		}

		/// <summary>
		/// Starts the hosted component.
		/// </summary>
		protected override void Initialize()
		{
			_host.StartComponent();

			base.Initialize();
		}

		/// <summary>
		/// Stops the hosted component.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _host != null)
			{
				_host.StopComponent();
				_host = null;
				_workspace = null;
			}
		}

		/// <summary>
		/// Creates a view for this dialog box.
		/// </summary>
		protected sealed override IDesktopObjectView CreateView()
		{
			return _workspace.CreateWorkspaceDialogBoxView(this);
		}

		#endregion

		#region Helpers


		/// <summary>
		/// Gets the view for this object as an <see cref="IWorkspaceDialogBoxView"/>.
		/// </summary>
		protected IWorkspaceDialogBoxView WorkspaceDialogBoxView
		{
			get { return (IWorkspaceDialogBoxView)this.View; }
		}

		#endregion

		#region IWorkspace Members

		/// <summary>
		/// Gets the workspace that owns this dialog box.
		/// </summary>
		IWorkspace IWorkspaceDialogBox.Workspace
		{
			get { return _workspace; }
		}

		#endregion

	}
}
