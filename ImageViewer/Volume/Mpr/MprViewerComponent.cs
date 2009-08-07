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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Tools;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprViewerComponent : ImageViewerComponent
	{
		#region Private fields

		private IToolSet _mprViewerToolSet;
		private IActionSet _mprViewerActionSet;

		private ObservableDisposableList<IMprVolume> _volumes;

		private string _title;

		#endregion

		public MprViewerComponent(Volume volume) : this()
		{
			_volumes.Add(new MprVolume(volume));
		}

		public MprViewerComponent(IMprVolume volume) : this()
		{
			_volumes.Add(volume);
		}

		public MprViewerComponent() : base(new MprLayoutManager(), null)
		{
			_volumes = new ObservableDisposableList<IMprVolume>();
			_volumes.EnableEvents = true;
		}

		public new IObservableList<IMprVolume> StudyTree
		{
			get { return _volumes; }
		}

		public string Title
		{
			get
			{
				if (_title == null)
					_title = this.SuggestTitle();
				return _title;
			}
			set
			{
				if (_title != value)
				{
					_title = value;
					base.NotifyPropertyChanged("Title");
				}
			}
		}

		protected virtual string SuggestTitle()
		{
			return StringUtilities.Combine(this.StudyTree, String.Format(" {0} ", SR.VolumeLabelSeparator), delegate(IMprVolume volume) { return volume.Description; });
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
			if (action.ActionID.StartsWith("ClearCanvas.ImageViewer.Tools.Reporting."))
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
				if (_volumes != null)
				{
					_volumes.Dispose();
					_volumes = null;
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

			protected override sealed IDisplaySet CreateDisplaySet(Series series)
			{
				throw new NotSupportedException();
			}

			protected override sealed IImageSet CreateImageSet(Study study)
			{
				throw new NotSupportedException();
			}

			protected virtual IDisplaySet CreateDisplaySet(int number, IMprSliceSet sliceSet)
			{
				string name;
				if (sliceSet is IMprStandardSliceSet && ((IMprStandardSliceSet)sliceSet).IsReadOnly)
					name = string.Format(SR.FormatMprDisplaySetName, sliceSet.Description);
				else
					name = string.Format(SR.FormatMprDisplaySetName, number - 1);

				DisplaySet displaySet = new MprDisplaySet(name, sliceSet);
				displaySet.Description = name;
				displaySet.Number = number;
				return displaySet;
			}

			protected virtual IImageSet CreateImageSet(MprVolume volume)
			{
				int number = 0;
				ImageSet imageSet = new ImageSet();
				foreach (IMprSliceSet sliceSet in volume.SliceSets)
				{
					imageSet.DisplaySets.Add(CreateDisplaySet(++number, sliceSet));
				}
				imageSet.Name = volume.Description;
				return imageSet;
			}

			protected override void BuildLogicalWorkspace()
			{
				foreach (MprVolume volume in this.ImageViewer.StudyTree)
				{
					this.ImageViewer.LogicalWorkspace.ImageSets.Add(CreateImageSet(volume));
				}
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
				// Do our own filling. The base method clones the display set, which is:
				// 1. Time consuming, because of the header generation
				// 2. Useless, because it can never be shown, and the workspace is locked anyway so you don't need to "recover" the original
				// 3. Makes reslicing slow, since you generate two sets of presentation images
				// 4. All of the above

				IPhysicalWorkspace physicalWorkspace = ImageViewer.PhysicalWorkspace;
				ILogicalWorkspace logicalWorkspace = ImageViewer.LogicalWorkspace;

				if (logicalWorkspace.ImageSets.Count == 0)
					return;

				int imageSetIndex = 0;
				int displaySetIndex = 0;

				foreach (IImageBox imageBox in physicalWorkspace.ImageBoxes)
				{
					if (displaySetIndex == logicalWorkspace.ImageSets[imageSetIndex].DisplaySets.Count)
					{
						imageSetIndex++;
						displaySetIndex = 0;

						if (imageSetIndex == logicalWorkspace.ImageSets.Count)
							break;
					}

					imageBox.DisplaySet = logicalWorkspace.ImageSets[imageSetIndex].DisplaySets[displaySetIndex];
					displaySetIndex++;
				}

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