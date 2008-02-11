using System.Collections.Generic;
using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
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
		private static readonly string _compositeGraphicName = "SynchronizationTools";
		
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

			OnReferenceLinesCalculated(CalculateReferenceLines());
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			if (_stackingSynchronizationTool != null)
				OnSynchronizedImages(_stackingSynchronizationTool.SynchronizeWithSelectedImage());
		}

		private static CompositeGraphic GetSynchronizationToolCompositeGraphic(IPresentationImage image)
		{
			if (image != null && image is INamedCompositeGraphicProvider)
			{
				CompositeGraphic container = ((INamedCompositeGraphicProvider) image).GetNamedCompositeGraphic(_compositeGraphicName);
				if (container != null)
				{
					container.Visible = true;
					return container;
				}
			}

			return null;
		}

		private IEnumerable<IPresentationImage> ClearReferencePointGraphics()
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
				foreach (IPresentationImage image in _referenceLineTool.CalculateReferenceLines())
					yield return image;
			}
			else
			{
				yield break;
			}
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

			// insert it at the beginning, because we want the spatial locator graphic on top.
			if (container.Graphics.Count == 0 || !(container.Graphics[0] is ReferenceLineCompositeGraphic))
				container.Graphics.Insert(0, new ReferenceLineCompositeGraphic());

			return (ReferenceLineCompositeGraphic)container.Graphics[0];
		}

		public SpatialLocatorGraphic GetSpatialLocatorGraphic(IPresentationImage image)
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

			existingGraphic.SpatialTransform.Initialize();
			return existingGraphic;
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

		public void ClearReferencePoints()
		{
			Draw(ClearReferencePointGraphics());
		}

		public void OnReferenceLinesCalculated(IEnumerable<IPresentationImage> affectedImages)
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
