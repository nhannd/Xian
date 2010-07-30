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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[MenuAction("Open", "explorerlocal-contextmenu/MenuOpenInStudyFilters", "Open")]
	[Tooltip("Open", "TooltipOpenInStudyFilters")]
	[IconSet("Open", IconScheme.Colour, "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (LocalImageExplorerToolExtensionPoint))]
	public class LaunchStudyFiltersLocalExplorerTool : Tool<ILocalImageExplorerToolContext>
	{
		public event EventHandler EnabledChanged;
		private bool _enabled = true;

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedPathsChanged += OnContextSelectedPathsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Context.SelectedPathsChanged -= OnContextSelectedPathsChanged;
			base.Dispose(disposing);
		}

		private void OnContextSelectedPathsChanged(object sender, EventArgs e)
		{
			Enabled = Context.SelectedPaths.Count > 0;
		}

		public void Open()
		{
			List<string> paths = new List<string>(base.Context.SelectedPaths);
			if (paths.Count == 0)
				return;

			StudyFilterComponent component = new StudyFilterComponent();
			component.BulkOperationsMode = true;

			if (component.Load(base.Context.DesktopWindow, true, paths))
			{
				component.Refresh(true);
				base.Context.DesktopWindow.Workspaces.AddNew(component, SR.TitleStudyFilters);
			}

			component.BulkOperationsMode = false;
		}
	}
}