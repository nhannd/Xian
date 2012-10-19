#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

				AuditHelper.DocumentWorkspaceOpened(this);

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
			return workspace != null && workspace.Close();
		}

		public abstract bool SaveAndClose();

		public abstract string GetTitle();

		public abstract IApplicationComponent GetComponent();

		/// <summary>
		/// Gets the audit data for opening this document, or null if auditing is not required.
		/// </summary>
		/// <returns></returns>
		public abstract OpenWorkspaceOperationAuditData GetAuditData();

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
