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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("activate", "dicomstudybrowser-contextmenu/StreamingAnalysis", "OpenAnalysisTool")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StreamingAnalysisTool : StudyBrowserTool
	{
		private IShelf _shelf;

		public StreamingAnalysisTool()
		{
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = true;
		}

		public override void Initialize()
		{
			base.Initialize();
			Enabled = true;
		}

		public void OpenAnalysisTool()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
			{
				StreamingAnalysisComponent component = new StreamingAnalysisComponent(base.Context);
				_shelf = ApplicationComponent.LaunchAsShelf(base.Context.DesktopWindow, component, 
					"Streaming Analysis", ShelfDisplayHint.DockFloat | ShelfDisplayHint.ShowNearMouse);

				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
