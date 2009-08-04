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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Volume.Mpr.Tools;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprViewerComponent : ImageViewerComponent
	{
		#region Private fields

		private IToolSet _mprViewerToolSet;
		private IActionSet _mprViewerActionSet;

		private Volume _volume;
		private MprDisplaySet _identityDisplaySet;
		private MprDisplaySet _orthoXDisplaySet;
		private MprDisplaySet _orthoYDisplaySet;
		private MprDisplaySet _obliqueDisplaySet;

		private string _title;

		#endregion

		public MprViewerComponent(Volume volume) : base(new MprLayoutManager(), null)
		{
			Platform.CheckForNullReference(volume, "volume");
			_volume = volume;
		}

		public string Title
		{
			get
			{
				if (_title == null)
					_title = _volume.Description;
				return _title;
			}
			set { _title = value; }
		}

		public override IActionSet ExportedActions
		{
			get { return _mprViewerActionSet; }
		}

		public override void Start()
		{
			base.Start();
			_mprViewerToolSet = new ToolSet(new MprViewerToolExtensionPoint(), new ToolContext(this));
			_mprViewerActionSet = _mprViewerToolSet.Actions.Union(base.ExportedActions);
		}

		public override void Stop()
		{
			_mprViewerActionSet = null;
			_mprViewerToolSet.Dispose();
			_mprViewerToolSet = null;
			base.Stop();
		}

		#region ToolContext

		private class ToolContext : IMprViewerToolContext
		{
			private readonly MprViewerComponent _viewer;

			public ToolContext(MprViewerComponent viewer)
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

		public MprDisplaySet IdentityDisplaySet
		{
			get
			{
				if (_identityDisplaySet == null)
					_identityDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.Identity, _volume);
				return _identityDisplaySet;
			}
		}

		public MprDisplaySet OrthoXDisplaySet
		{
			get
			{
				if (_orthoXDisplaySet == null)
					_orthoXDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.OrthoY, _volume);
				return _orthoXDisplaySet;
			}
		}

		public MprDisplaySet OrthoYDisplaySet
		{
			get
			{
				if (_orthoYDisplaySet == null)
					_orthoYDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.OrthoX, _volume);
				return _orthoYDisplaySet;
			}
		}

		public MprDisplaySet ObliqueDisplaySet
		{
			get
			{
				if (_obliqueDisplaySet == null)
					_obliqueDisplaySet = MprDisplaySet.Create(MprDisplaySetIdentifier.Oblique, _volume);
				return _obliqueDisplaySet;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_identityDisplaySet != null)
				{
					_identityDisplaySet.Dispose();
					_identityDisplaySet = null;
				}

				if (_orthoXDisplaySet != null)
				{
					_orthoXDisplaySet.Dispose();
					_orthoXDisplaySet = null;
				}

				if (_orthoYDisplaySet != null)
				{
					_orthoYDisplaySet.Dispose();
					_orthoYDisplaySet = null;
				}

				if (_obliqueDisplaySet != null)
				{
					_obliqueDisplaySet.Dispose();
					_obliqueDisplaySet = null;
				}

				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}

			base.Dispose(disposing);
		}

		private class MprLayoutManager : LayoutManager
		{
			public override void Layout()
			{
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

			protected override void FillPhysicalWorkspace()
			{
				// Let's start out in the middle of each stack
				IPhysicalWorkspace physicalWorkspace = this.ImageViewer.PhysicalWorkspace;
				physicalWorkspace.ImageBoxes[0].DisplaySet = this.ImageViewer.IdentityDisplaySet;
				physicalWorkspace.ImageBoxes[0].TopLeftPresentationImageIndex = this.ImageViewer.IdentityDisplaySet.PresentationImages.Count/2;
				physicalWorkspace.ImageBoxes[1].DisplaySet = this.ImageViewer.OrthoXDisplaySet;
				physicalWorkspace.ImageBoxes[1].TopLeftPresentationImageIndex = this.ImageViewer.OrthoXDisplaySet.PresentationImages.Count/2;
				physicalWorkspace.ImageBoxes[2].DisplaySet = this.ImageViewer.OrthoYDisplaySet;
				physicalWorkspace.ImageBoxes[2].TopLeftPresentationImageIndex = this.ImageViewer.OrthoYDisplaySet.PresentationImages.Count/2;
				physicalWorkspace.ImageBoxes[3].DisplaySet = this.ImageViewer.ObliqueDisplaySet;
				physicalWorkspace.ImageBoxes[3].TopLeftPresentationImageIndex = this.ImageViewer.ObliqueDisplaySet.PresentationImages.Count/2;
			}
		}
	}
}