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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled = true;
		private event EventHandler _enabledChangedEvent;

		private bool _visible = true;
		private event EventHandler _visibleChangedEvent;

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

		public bool Visible
		{
			get { return _visible; }
			protected set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler VisibleChanged
		{
			add { _visibleChangedEvent += value; }
			remove { _visibleChangedEvent -= value; }
		}
	}
}
