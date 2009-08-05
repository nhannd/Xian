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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Tools;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprViewerComponent : ImageViewerComponent
	{
		#region Private fields

		private IToolSet _mprViewerToolSet;
		private IActionSet _mprViewerActionSet;

		private IVolumeReference _volume;

		private string _title;

		#endregion

		public MprViewerComponent(Volume volume) : base(new MprLayoutManager(), null)
		{
			Platform.CheckForNullReference(volume, "volume");
			_volume = volume.CreateTransientReference();
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		public string Title
		{
			get
			{
				if (_title == null)
					_title = _volume.Volume.Description;
				return _title;
			}
			set { _title = value; }
		}

		public override IActionSet ExportedActions
		{
			get { return _mprViewerActionSet; }
		}

		public override ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			return ActionModelRoot.CreateModel(typeof (MprViewerComponent).FullName, "imageviewer-contextmenu", this.ExportedActions);
		}

		private static bool ImageViewerToolActionPredicate(IAction action)
		{
			// exclude all Layout tools because we have our own fixed layout manager
			if (action.ActionID.StartsWith("ClearCanvas.ImageViewer.Layout.Basic."))
				return false;

			// exclude all Reporting tools because we would need to store the secondary capture for any reporting to work
			if (action.ActionID.StartsWith("ClearCanvas.ImageViewer.Tools.Reporting"))
				return false;

			// default: include
			return true;
		}

		public override void Start()
		{
			base.Start();
			_mprViewerToolSet = new ToolSet(new MprViewerToolExtensionPoint(), new MprViewerToolContext(this));
			_mprViewerActionSet = _mprViewerToolSet.Actions.Union(base.ExportedActions.Select(ImageViewerToolActionPredicate));
		}

		public override void Stop()
		{
			_mprViewerActionSet = null;
			_mprViewerToolSet.Dispose();
			_mprViewerToolSet = null;
			base.Stop();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Tool Context

		private class MprViewerToolContext : IMprViewerToolContext
		{
			private readonly MprViewerComponent _viewer;

			public MprViewerToolContext(MprViewerComponent viewer)
			{
				_viewer = viewer;
			}

			public MprViewerComponent Viewer
			{
				get { return _viewer; }
			}

			IImageViewer IImageViewerToolContext.Viewer
			{
				get { return _viewer; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _viewer.DesktopWindow; }
			}
		}

		#endregion

		#region Layout Manager

		private class MprLayoutManager : LayoutManager
		{
			public override void Layout()
			{
				this.BuildLogicalWorkspace();
				this.LayoutPhysicalWorkspace();
				this.FillPhysicalWorkspace();
				this.ImageViewer.PhysicalWorkspace.Draw();
			}

			public new MprViewerComponent ImageViewer
			{
				get { return (MprViewerComponent) base.ImageViewer; }
			}

			protected override void LayoutPhysicalWorkspace()
			{
				base.ImageViewer.PhysicalWorkspace.SetImageBoxGrid(2, 2);

				foreach (IImageBox imageBox in base.ImageViewer.PhysicalWorkspace.ImageBoxes)
					imageBox.SetTileGrid(1, 1);

				base.ImageViewer.PhysicalWorkspace.Locked = true;
			}

			protected static IEnumerable<IVolumeSlicerParams> GetDefaultViews()
			{
				yield return VolumeSlicerParams.Identity;
				yield return VolumeSlicerParams.OrthogonalX;
				yield return VolumeSlicerParams.OrthogonalY;
				yield return new VolumeSlicerParams(90, 0, 45);
			}

			protected override void BuildLogicalWorkspace()
			{
				int number = 0;
				ImageSet imageSet = new ImageSet();
				foreach (IVolumeSlicerParams slicerParams in GetDefaultViews())
				{
					string name = string.Format(SR.FormatMprDisplaySetName, slicerParams.Description);
					MprDisplaySet displaySet = new MprDisplaySet(this.ImageViewer.Volume, slicerParams, name);
					displaySet.Description = name;
					displaySet.Number = ++number;
					imageSet.DisplaySets.Add(displaySet);
				}
				this.ImageViewer.LogicalWorkspace.ImageSets.Add(imageSet);
			}

			protected override void FillPhysicalWorkspace()
			{
				base.FillPhysicalWorkspace();

				// Let's start out in the middle of each stack
				foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
				{
					imageBox.TopLeftPresentationImageIndex = imageBox.DisplaySet.PresentationImages.Count/2;
				}
			}
		}

		#endregion
	}
}