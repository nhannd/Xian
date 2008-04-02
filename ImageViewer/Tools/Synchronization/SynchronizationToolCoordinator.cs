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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class SynchronizationToolCompositeGraphic : CompositeGraphic
	{
		public SynchronizationToolCompositeGraphic()
		{
			base.Graphics.Add(new ReferenceLineCompositeGraphic());
		}
	}

	// NOTE: Because the synchronization tools act based on events coming from the viewer event broker,
	// they cannot be responsible for drawing the images because that would result in repeated (unnecessary)
	// Draw() calls.  The reference line tool is dependent on the stacking synchronization tool in that 
	// it needs to synchronize the images first, before the reference lines are drawn, otherwise images
	// that are about to become invisible will be unnecessarily drawn.
	//
	// This class acts as a mediator, listening to events from the viewer event broker and coordinating
	// the synchronization tools (the order they are called in, as well as drawing the affected images
	// such that none are redrawn unnecessarily).
	//
	// There may be a more general solution for coordinating draws, but this problem may be 
	// more or less isolated to synchronization tools, so we will just use this class for
	// now rather than spend the time to develop a general solution unnecessarily.

	internal class SynchronizationToolCoordinator
	{
		private static readonly Dictionary<IImageViewer, SynchronizationToolCoordinator> _coordinators;

		private readonly IImageViewer _viewer;
		
		private StackingSynchronizationTool _stackingSynchronizationTool;
		
		private ReferenceLineTool _referenceLineTool;

		private SpatialLocatorTool _spatialLocatorTool;
		private readonly List<SpatialLocatorGraphic> _spatialLocatorGraphicCache;

		private int _referenceCount;

		static SynchronizationToolCoordinator()
		{
			_coordinators = new Dictionary<IImageViewer, SynchronizationToolCoordinator>();
		}

		private SynchronizationToolCoordinator(IImageViewer viewer)
		{
			_viewer = viewer;
			_spatialLocatorGraphicCache = new List<SpatialLocatorGraphic>();
		}

		public StackingSynchronizationTool StackingSynchronizationTool
		{
			set { _stackingSynchronizationTool = value; }
		}

		public ReferenceLineTool ReferenceLineTool
		{
			set { _referenceLineTool = value; }
		}

		public SpatialLocatorTool SpatialLocatorTool
		{
			set { _spatialLocatorTool = value; }	
		}

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage != null)
			{
				//the presentation image selected event will fire and take care of this.
				return;
			}

			OnReferenceLinesRefreshed(CalculateReferenceLines());
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			if (_stackingSynchronizationTool != null)
				OnSynchronizedImages(_stackingSynchronizationTool.SynchronizeWithSelectedImage());
		}

		private static CompositeGraphic GetSynchronizationToolCompositeGraphic(IPresentationImage image)
		{
			if (image is IOverlayGraphicsProvider)
			{
				GraphicCollection overlayGraphics = ((IOverlayGraphicsProvider) image).OverlayGraphics;

				IGraphic container = CollectionUtils.SelectFirst(overlayGraphics, delegate(IGraphic graphic)
		                                             	{
		                                             		return graphic is SynchronizationToolCompositeGraphic;
		                                             	}); 
				if (container == null)
					overlayGraphics.Insert(0, container = new SynchronizationToolCompositeGraphic());

				container.Visible = true;
				return (CompositeGraphic)container;
			}

			return null;
		}

		private IEnumerable<IPresentationImage> ClearSpatialLocatorGraphics()
		{
			foreach (SpatialLocatorGraphic graphic in _spatialLocatorGraphicCache)
			{
				IPresentationImage parentImage = graphic.ParentPresentationImage;
				CompositeGraphic parentGraphic = graphic.ParentGraphic as CompositeGraphic;

				if (parentGraphic != null)
				{
					parentGraphic.Graphics.Remove(graphic);

					if (parentImage != null)
						yield return parentImage;
				}

				graphic.Dispose();

			}

			_spatialLocatorGraphicCache.Clear();
		}

		private static void Draw <T>(IEnumerable<T> itemsToDraw)
			where T : IDrawable
		{
			foreach (IDrawable drawable in itemsToDraw)
				drawable.Draw();
		}

		private IEnumerable<IPresentationImage> CalculateReferenceLines()
		{
			if (_referenceLineTool != null)
			{
				foreach (IPresentationImage image in _referenceLineTool.GetRefreshedImages())
					yield return image;
			}
			else
			{
				yield break;
			}
		}

		private SpatialLocatorGraphic GetSpatialLocatorGraphic(IPresentationImage image, bool findOnly)
		{
			CompositeGraphic container = GetSynchronizationToolCompositeGraphic(image);
			if (container == null)
				return null;

			SpatialLocatorGraphic existingGraphic = _spatialLocatorGraphicCache.Find(delegate(SpatialLocatorGraphic graphic)
										{
											if (graphic.ParentPresentationImage == null)
												return false;

											return graphic.ParentPresentationImage.ParentDisplaySet.ImageBox == image.ParentDisplaySet.ImageBox;
										});

			if (findOnly)
				return existingGraphic;

			if (existingGraphic == null)
			{
				existingGraphic = _spatialLocatorGraphicCache.Find(delegate(SpatialLocatorGraphic graphic)
										{
											return graphic.ParentPresentationImage == null;
										});

				if (existingGraphic != null)
				{
					container.Graphics.Add(existingGraphic);
				}
			}
			else
			{
				if (existingGraphic.ParentPresentationImage != image)
				{
					((CompositeGraphic)existingGraphic.ParentGraphic).Graphics.Remove(existingGraphic);
					container.Graphics.Add(existingGraphic);
				}
			}

			if (existingGraphic == null)
			{
				_spatialLocatorGraphicCache.Add(existingGraphic = new SpatialLocatorGraphic());
				container.Graphics.Add(existingGraphic);
			}

			return existingGraphic;
		}

		public static SynchronizationToolCoordinator Get(IImageViewer viewer)
		{
			if (!_coordinators.ContainsKey(viewer))
			{
				SynchronizationToolCoordinator coordinator = new SynchronizationToolCoordinator(viewer);

				viewer.EventBroker.PresentationImageSelected += coordinator.OnPresentationImageSelected;
				viewer.EventBroker.TileSelected += coordinator.OnTileSelected;
			
				_coordinators.Add(viewer, coordinator);
			}

			++_coordinators[viewer]._referenceCount;
			return _coordinators[viewer];
		}

		public void Release()
		{
			--_referenceCount;
			if (_referenceCount <= 0)
			{
				_viewer.EventBroker.PresentationImageSelected -= OnPresentationImageSelected;
				_viewer.EventBroker.TileSelected -= OnTileSelected;

				_coordinators.Remove(_viewer);
			}
		}

		public ReferenceLineCompositeGraphic GetReferenceLineCompositeGraphic(IPresentationImage image)
		{
			CompositeGraphic container = GetSynchronizationToolCompositeGraphic(image);
			if (container == null)
				return null;

			return (ReferenceLineCompositeGraphic)container.Graphics[0];
		}

		public SpatialLocatorGraphic ShowSpatialLocatorGraphic(IPresentationImage image)
		{
			return GetSpatialLocatorGraphic(image, false);
		}

		public bool HideSpatialLocatorGraphic(IPresentationImage image)
		{
			SpatialLocatorGraphic existingGraphic = GetSpatialLocatorGraphic(image, true);
			if (existingGraphic != null)
			{
				((CompositeGraphic)existingGraphic.ParentGraphic).Graphics.Remove(existingGraphic);
				return true;
			}

			return false;
		}

		public void OnSpatialLocatorPointsCalculated(IEnumerable<IImageBox> affectedImageBoxes)
		{
			//The spatial locator and stacking sync tool conflict.
			_stackingSynchronizationTool.SynchronizeActive = false;

			//Sync up the images first.
			List<IImageBox> imageBoxesToDraw = new List<IImageBox>(affectedImageBoxes);

			//Then calculate the reference lines.
			foreach (IPresentationImage image in CalculateReferenceLines())
			{
				//Only draw images that won't be drawn as a result of the image boxes being drawn.
				if (!imageBoxesToDraw.Contains(image.ParentDisplaySet.ImageBox))
					image.Draw();
			}

			Draw(imageBoxesToDraw);
		}

		public void OnSpatialLocatorStopped()
		{
			Draw(ClearSpatialLocatorGraphics());
		}

		public void OnReferenceLinesRefreshed(IEnumerable<IPresentationImage> affectedImages)
		{
			Draw(affectedImages);
		}

		public void OnSynchronizedImages(IEnumerable<IImageBox> affectedImageBoxes)
		{
			List<IImageBox> imageBoxesToDraw = new List<IImageBox>(affectedImageBoxes);

			foreach (IPresentationImage image in CalculateReferenceLines())
			{
				if (!imageBoxesToDraw.Contains(image.ParentDisplaySet.ImageBox))
					image.Draw();
			}

			Draw(imageBoxesToDraw);
		}
	}
}
