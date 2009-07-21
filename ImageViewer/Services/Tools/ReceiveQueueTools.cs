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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ButtonAction("clearSelected", "receive-queue-toolbar/ClearSelected", "ClearSelected")]
	[MenuAction("clearSelected", "receive-queue-contextmenu/MenuClear", "ClearSelected")]
	[Tooltip("clearSelected", "TooltipClearSelected")]
	[IconSet("clearSelected", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png", "Icons.DeleteToolSmall.png")]
	[EnabledStateObserver("clearSelected", "ClearSelectedEnabled", "ClearSelectedEnabledChanged")]

	[ButtonAction("clearAll", "receive-queue-toolbar/ClearAll", "ClearAll")]
	[Tooltip("clearAll", "TooltipClearAll")]
	[IconSet("clearAll", IconScheme.Colour, "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png", "Icons.DeleteAllToolSmall.png")]
	[EnabledStateObserver("clearAll", "ClearAllEnabled", "ClearAllEnabledChanged")]

	[ButtonAction("openStudies", "receive-queue-toolbar/ToolbarOpenStudies", "OpenStudies")]
	[MenuAction("openStudies", "receive-queue-contextmenu/MenuOpenStudies", "OpenStudies")]
	[Tooltip("openStudies", "TooltipOpenStudies")]
	[IconSet("openStudies", IconScheme.Colour, "Icons.OpenStudiesToolSmall.png", "Icons.OpenStudiesToolSmall.png", "Icons.OpenStudiesToolSmall.png")]
	[EnabledStateObserver("openStudies", "OpenStudiesEnabled", "OpenStudiesEnabledChanged")]
	[ViewerActionPermission("openStudies", ImageViewer.AuthorityTokens.Study.Open)]

	[ExtensionOf(typeof(ReceiveQueueApplicationComponentToolExtensionPoint))]
	public class ReceiveQueueTools : Tool<IReceiveQueueApplicationComponentToolContext>
	{
		private bool _clearSelectedEnabled;
		private bool _clearAllEnabled;
		private bool _openStudiesEnabled;

		private event EventHandler _clearSelectedEnabledChanged;
		private event EventHandler _clearAllEnabledChanged;
		private event EventHandler _openStudiesEnabledChanged;

		public ReceiveQueueTools()
		{
			_clearSelectedEnabled = false;
			_clearAllEnabled = false;
			_openStudiesEnabled = false;
		}

		private void OnUpdated(object sender, EventArgs e)
		{
			this.ClearAllEnabled = this.Context.NumberOfItems > 0;
			this.ClearSelectedEnabled = this.Context.NumberSelected > 0;
			this.OpenStudiesEnabled = this.Context.NumberSelected > 0 && 
										CollectionUtils.TrueForAll(this.Context.SelectedItems,
	                                                     delegate(ReceiveQueueItem item)
	                                                     	{
																return item.NumberOfFilesCommittedToDataStore > 0;
	                                                     	});
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.Updated += OnUpdated;
			this.Context.DefaultActionHandler = OpenStudies;
		}

		public bool ClearSelectedEnabled
		{
			get { return _clearSelectedEnabled; }
			protected set
			{
				if (_clearSelectedEnabled != value)
				{
					_clearSelectedEnabled = value;
					EventsHelper.Fire(_clearSelectedEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool ClearAllEnabled
		{
			get { return _clearAllEnabled; }
			protected set
			{
				if (_clearAllEnabled != value)
				{
					_clearAllEnabled = value;
					EventsHelper.Fire(_clearAllEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public bool OpenStudiesEnabled
		{
			get { return _openStudiesEnabled; }
			protected set
			{
				if (_openStudiesEnabled != value)
				{
					_openStudiesEnabled = value;
					EventsHelper.Fire(_openStudiesEnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler ClearSelectedEnabledChanged
		{
			add { _clearSelectedEnabledChanged += value; }
			remove { _clearSelectedEnabledChanged -= value; }
		}

		public event EventHandler ClearAllEnabledChanged
		{
			add { _clearAllEnabledChanged += value; }
			remove { _clearAllEnabledChanged -= value; }
		}

		public event EventHandler OpenStudiesEnabledChanged
		{
			add { _openStudiesEnabledChanged += value; }
			remove { _openStudiesEnabledChanged -= value; }
		}

		private void ClearSelected()
		{
			try
			{
				this.Context.ClearSelected();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void ClearAll()
		{
			try
			{
				this.Context.ClearAll();

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageCancelFailed, this.Context.DesktopWindow);
			}
		}

		private void OpenStudies()
		{
			if (!OpenStudiesEnabled)
				return;

			try
			{
				OpenStudyHelper helper = new OpenStudyHelper();
				helper.WindowBehaviour = ViewerLaunchSettings.WindowBehaviour;

				foreach (ReceiveQueueItem item in Context.SelectedItems)
					helper.AddStudy(item.StudyInformation.StudyInstanceUid, null, "DICOM_LOCAL");

				helper.OpenStudies();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
