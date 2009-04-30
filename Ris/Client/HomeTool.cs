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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// A tool for launching a home page.  A home page consists of a set of folder systems and a preview component.
	/// </summary>
	/// <remarks>
	/// Subclasses of this class should specify a <see cref="MenuActionAttribute"/> attribute with the Launch method as the clickHandler
	/// </remarks>
	public abstract class HomeTool : Tool<IDesktopToolContext> 
	{
		private IWorkspace _workspace;

		/// <summary>
		/// Title displayed when the home page is active
		/// </summary>
		public abstract string Title { get; }

		/// <summary>
		/// Creates the homepage component.
		/// </summary>
		/// <returns></returns>
		protected abstract IApplicationComponent CreateComponent();

		/// <summary>
		/// Determines if the workspace that is launched is user-closable or not.
		/// </summary>
		protected virtual bool IsUserClosableWorkspace
		{
			get { return true; }
		}

		/// <summary>
		/// Default clickHandler implementation for <see cref="MenuAction"/> and/or <see cref="ButtonAction"/> attributes.
		/// These attributes must be specified on subclasses.
		/// </summary>
		public void Launch()
		{
			if (_workspace == null)
			{
				Open();
			}
			else
			{
				_workspace.Activate();
			}
		}

		/// <summary>
		/// Re-starts the homepage, close any existing open homepage.
		/// </summary>
		protected void Restart()
		{
			if (_workspace != null)
			{
				_workspace.Close();
				_workspace = null;
			}

			Open();
		}

		private void Open()
		{
			try
			{
				IApplicationComponent component = CreateComponent();

				if (component != null)
				{
					WorkspaceCreationArgs args = new WorkspaceCreationArgs(component, this.Title, null);
					args.UserClosable = this.IsUserClosableWorkspace;
					_workspace = ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, args);
					_workspace.Closed += delegate { _workspace = null; };
				}
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// A tool for launching a home page with a <see cref="WorklistItemPreviewComponent"/> as the preview component
	/// </summary>
	/// <typeparam name="TFolderSystemExtensionPoint">Specifies the extension point used to create the set of folder systems</typeparam>
	public abstract class WorklistPreviewHomeTool<TFolderSystemExtensionPoint> : HomeTool
		where TFolderSystemExtensionPoint : ExtensionPoint<IFolderSystem>, new()
	{
		protected override IApplicationComponent  CreateComponent()
		{
			List<IFolderSystem> folderSystems = GetFolderSystems();
			if (folderSystems.Count == 0)
				return null;

			// Find all the folder systems
			WorklistItemPreviewComponent previewComponent = new WorklistItemPreviewComponent();
			return new HomePageContainer(folderSystems, previewComponent);
		}

		protected bool HasFolderSystems
		{
			get { return GetFolderSystems().Count > 0; }
		}

		private List<IFolderSystem> GetFolderSystems()
		{
			return CollectionUtils.Cast<IFolderSystem>(new TFolderSystemExtensionPoint().CreateExtensions());
		}
	}
}
