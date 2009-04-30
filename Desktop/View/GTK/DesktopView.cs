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
using ClearCanvas.Common;
//using ClearCanvas.Workstation.Model;
using ClearCanvas.Desktop;
using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	//[ExtensionOf(typeof(ClearCanvas.Workstation.Model.DesktopViewExtensionPoint))]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopViewExtensionPoint))]
	//public class WorkstationView : GtkView, IWorkstationView
	public class DesktopView : GtkView, IDesktopView
	{
		private static MainWindow _mainWin;

		//public WorkstationView()
		public DesktopView()
		{
		}
		
		private void Initialize()
		{
            Gtk.Application.Init();
            _mainWin = new MainWindow();
			_mainWin.DeleteEvent += OnDeleteMainWindow;
		}
		
		internal MainWindow MainWindow
		{
			get { return _mainWin; }
		}
		
		/// <summary>
		/// Starts the message pump of the underlying GUI toolkit.  Typically this method is expected to
		/// block for the duration of the application's execution.
		/// </summary>
		/// <remarks>
		/// The method assumes that the view relies on an underlying message pump, as most
		/// desktop GUI toolkits do.  This may need to change if a non-desktop (ie web) view
		/// is implemented.
		/// </remarks>
		public void RunMessagePump()
		{
			Initialize();
			Gtk.Application.Run();
		}
		
		/// <summary>
		/// Stops the underlying message pump, typically just prior to the termination of the application.
		/// </summary>
		public void QuitMessagePump()
		{
			Gtk.Application.Quit();
		}
		
		///<summary>
		/// Returns the GTK widget that implements this view, allowing a parent view to insert
		/// the widget as one of its children.
		/// </summary>
		public override object GuiElement
		{
			get { return _mainWin; }
		}
		
		public void CloseActiveWorkspace()
        {
           IWorkspace workspace = _mainWin.ActiveWorkspace;
			if(workspace != null)
			{
				//WorkstationModel.WorkspaceManager.Workspaces.Remove(workspace);
				DesktopApplication.WorkspaceManager.Workspaces.Remove(workspace);
			}
        }

		// Handles the main window close event
		private void OnDeleteMainWindow(object sender, EventArgs e)
        {
			QuitMessagePump();
        }
		
	}
}
