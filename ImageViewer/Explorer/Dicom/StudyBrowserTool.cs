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
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		protected const string LocalStudyLoaderName = "DICOM_LOCAL";
		protected const string RemoteStudyLoaderName = "DICOM_REMOTE";
		protected const string StreamingStudyLoaderName = "CC_STREAMING";

		public StudyBrowserTool()
		{

		}

		protected bool IsLocalStudyLoaderSupported
		{
			get { return ImageViewerComponent.IsStudyLoaderSupported(LocalStudyLoaderName); }
		}

		protected bool IsStreamingStudyLoaderSupported
		{
			get { return ImageViewerComponent.IsStudyLoaderSupported(StreamingStudyLoaderName); }
		}

		protected bool IsRemoteStudyLoaderSupported
		{
			get { return ImageViewerComponent.IsStudyLoaderSupported(RemoteStudyLoaderName); }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			this.Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		protected virtual void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (this.Context.SelectedStudy != null)
				this.Enabled = true;
			else
				this.Enabled = false;
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}
	}
}
