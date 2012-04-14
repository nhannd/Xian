#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		protected const string LocalStudyLoaderName = "DICOM_LOCAL";
		protected const string RemoteStudyLoaderName = "DICOM_REMOTE";
		protected const string StreamingStudyLoaderName = "CC_STREAMING";

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
			protected set
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
