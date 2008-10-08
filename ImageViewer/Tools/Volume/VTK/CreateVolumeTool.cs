#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	[ButtonAction("show", "global-toolbars/ToolbarsVolume/CreateVolumeTool", "Show")]
	[Tooltip("show", "Create Volume")]
	[IconSet("show", IconScheme.Colour, "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolMedium.png", "Icons.CreateVolumeToolLarge.png")]
	[GroupHint("show", "Tools.VolumeImage.Create")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class CreateVolumeTool : ImageViewerTool
	{
		private static VolumeComponent _volumeComponent;
		private static IShelf _volumeShelf;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public CreateVolumeTool()
		{
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		public void Show()
		{
			if (_volumeComponent == null)
			{
				// create and initialize the layout component
				_volumeComponent = new VolumeComponent(this.Context.DesktopWindow);

				// launch the layout component in a shelf
				_volumeShelf = ApplicationComponent.LaunchAsShelf(
					this.Context.DesktopWindow,
					_volumeComponent,
					SR.TitleVolumeController,
					ShelfDisplayHint.DockLeft);

				_volumeShelf.Closed += VolumeShelf_Closed;
			}
		}

		private static void VolumeShelf_Closed(object sender, ClosedEventArgs e) {
			// note that the component is thrown away when the shelf is closed by the user
			_volumeShelf.Closed -= VolumeShelf_Closed;
			_volumeShelf = null;
			_volumeComponent = null;
		}
	}
}
