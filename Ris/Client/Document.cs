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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public abstract class Document
	{
		private readonly string _key;
		private readonly IDesktopWindow _desktopWindow;
		private event EventHandler<ClosedEventArgs> _closed;

		protected Document(EntityRef subject, IDesktopWindow desktopWindow)
		{
			_key = DocumentManager.GenerateDocumentKey(this, subject);
			_desktopWindow = desktopWindow;
		}

		public string Key
		{
			get { return _key; }
		}

		public void Open()
		{
			var workspace = GetWorkspace(_key);
			if (workspace != null)
			{
				workspace.Activate();
			}
			else
			{
				workspace = LaunchWorkspace();
				if (workspace != null)
				{
					workspace.Closed += DocumentClosedEventHandler;
					DocumentManager.RegisterDocument(this);
				}
			}

		}

		public bool Close()
		{
			var workspace = GetWorkspace(_key);
			return workspace == null ? false : workspace.Close();
		}

		public abstract bool SaveAndClose();

		public abstract string GetTitle();

		public abstract IApplicationComponent GetComponent();

		public event EventHandler<ClosedEventArgs> Closed
		{
			add { _closed += value; }
			remove { _closed -= value; }
		}

		#region Private Helpers

		private void DocumentClosedEventHandler(object sender, ClosedEventArgs e)
		{
			DocumentManager.UnregisterDocument(this);
			EventsHelper.Fire(_closed, this, e);
		}

		private Workspace LaunchWorkspace()
		{
			Workspace workspace = null;

			try
			{
				workspace = ApplicationComponent.LaunchAsWorkspace(
					_desktopWindow,
					GetComponent(),
					GetTitle(),
					_key);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, _desktopWindow);
			}

			return workspace;
		}

		private static Workspace GetWorkspace(string documentKey)
		{
			foreach (var window in Desktop.Application.DesktopWindows)
			{
				if (!string.IsNullOrEmpty(documentKey) && window.Workspaces.Contains(documentKey))
					return window.Workspaces[documentKey];
			}

			return null;
		}

		#endregion
	}
}
