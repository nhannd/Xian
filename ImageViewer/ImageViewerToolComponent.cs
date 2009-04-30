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

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A base class for image viewer related application components.
	/// </summary>
	/// <remarks>
	/// There is often a need for an <see cref="ApplicationComponent"/> that will
	/// listen for changes in <see cref="ImageViewerComponent"/>activation.  
	/// For example, the shelf that appears when the image layout tool activated 
	/// has UI elements which should be enabled/disabled depending on whether the 
	/// active workspace contains an <see cref="ImageViewerComponent"/>.
	/// This base class encapsulates that functionality.  If you are developing an 
	/// <see cref="ApplicationComponent"/> that requires that kind of functionality,
	/// use this as your base class.
	/// </remarks>
	public abstract class ImageViewerToolComponent : ApplicationComponent
	{
		private IDesktopWindow _desktopWindow;
		private IImageViewer _imageViewer;

		/// <summary>
		/// Initializes a new instance of <see cref="ImageViewerToolComponent"/>.
		/// </summary>
		protected ImageViewerToolComponent(IDesktopWindow desktopWindow)
		{
			Platform.CheckForNullReference(desktopWindow, "desktopWindow");
			_desktopWindow = desktopWindow;
			ImageViewer = ImageViewerComponent.GetAsImageViewer(_desktopWindow.ActiveWorkspace);
		}

		/// <summary>
		/// Gets the <see cref="IDesktopWindow"/> associated with this object.
		/// </summary>
		public IDesktopWindow DesktopWindow
		{
			get { return _desktopWindow; }	
		}

		/// <summary>
		/// Gets the subject <see cref="IImageViewer"/> that this component is associated with.
		/// </summary>
		/// <remarks>
		/// <value>
		/// The currently active <see cref="IImageViewer"/> or <b>null</b> if the
		/// there is no currently active <see cref="IImageViewer"/>.
		/// </value>
		/// </remarks>
		public IImageViewer ImageViewer
		{
			get { return _imageViewer; }
			private set
			{
				if (value != _imageViewer)
				{
					ActiveImageViewerChangedEventArgs e = new ActiveImageViewerChangedEventArgs(value, _imageViewer);

					try
					{
						OnActiveImageViewerChanging(e);
					}
					finally
					{
						_imageViewer = value;
						OnActiveImageViewerChanged(e);
					}
				}
			}
		}

		/// <summary>
		/// Called when the active image viewer is about to change.
		/// </summary>
		protected virtual void OnActiveImageViewerChanging(ActiveImageViewerChangedEventArgs e)
		{
		}

		/// <summary>
		/// Called when the active image viewer has changed.
		/// </summary>
		protected virtual void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
		}

		#region ApplicationComponent overrides

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Start"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Start()
		{
			base.Start();

			_desktopWindow.Workspaces.ItemActivationChanged += OnWorkspaceActivated;
		}

		/// <summary>
		/// Override of <see cref="ApplicationComponent.Stop"/>
		/// </summary>
		/// <remarks>
		/// For internal Framework use only.
		/// </remarks>
		public override void Stop()
		{
			_desktopWindow.Workspaces.ItemActivationChanged -= OnWorkspaceActivated;

			base.Stop();
		}

		#endregion

		private void OnWorkspaceActivated(object sender, ItemEventArgs<Workspace> e)
		{
			Workspace activeWorkspace = _desktopWindow.ActiveWorkspace;

			if (activeWorkspace == null)
			{
				this.ImageViewer = null;
			}
			else
			{
				IImageViewer imageViewer = ImageViewerComponent.GetAsImageViewer(activeWorkspace);
				this.ImageViewer = imageViewer;
			}
		}
	}
}
