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

using System.Collections.Generic;
using System.Threading;
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
	//[IconSet("Open", IconScheme.Colour, "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
	[ExtensionOf(typeof (LocalImageExplorerToolExtensionPoint))]
	public class LocalExplorerTool : Tool<ILocalImageExplorerToolContext>
	{
		private SynchronizationContext _syncContext;

		public void Open()
		{
			if (_syncContext == null)
				_syncContext = SynchronizationContext.Current;

			List<string> paths = new List<string>();
			foreach (string path in base.Context.SelectedPaths)
				paths.Add(path);

			BackgroundTask task = new BackgroundTask(this.LoadFilterComponent, false, paths.AsReadOnly());
			ProgressDialog.Show(task, base.Context.DesktopWindow, true, ProgressBarStyle.Marquee);
		}

		private void LoadFilterComponent(IBackgroundTaskContext context)
		{
			IList<string> selectedPaths = context.UserState as IList<string>;
			if (selectedPaths == null)
				return;

			StudyFilterComponent component = new StudyFilterComponent();
			component.Load(selectedPaths);
			//for (int n = 0; n < selectedPaths.Count; n++)
			//{
			//    context.ReportProgress(new BackgroundTaskProgress(n, selectedPaths.Count, "SR.MessageLoading..."));
			//    component.Load(selectedPaths[n]);
			//}
			component.Refresh();

			_syncContext.Send(this.AddNewWorkspace, component);
		}

		private void AddNewWorkspace(object component)
		{
			base.Context.DesktopWindow.Workspaces.AddNew((IApplicationComponent) component, SR.StudyFilters);
		}
	}
}