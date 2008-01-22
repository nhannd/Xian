using System.Collections.Generic;

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
		private static readonly Dictionary<IImageViewer, SynchronizationToolCoordinator> _coordinators;

		private readonly IImageViewer _viewer;
		private StackingSynchronizationTool _stackingSynchronizationTool;
		private ReferenceLineTool _referenceLineTool;
		private int _referenceCount;

		static SynchronizationToolCoordinator()
		{
			_coordinators = new Dictionary<IImageViewer, SynchronizationToolCoordinator>();
		}

		private SynchronizationToolCoordinator(IImageViewer viewer)
		{
			_viewer = viewer;
		}

		public StackingSynchronizationTool StackingSynchronizationTool
		{
			set { _stackingSynchronizationTool = value; }
		}

		public ReferenceLineTool ReferenceLineTool
		{
			set { _referenceLineTool = value; }
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

		public void OnReferenceLinesCalculated(IEnumerable<IPresentationImage> affectedImages)
		{
			foreach (IPresentationImage image in affectedImages)
				image.Draw();
		}

		public void OnSynchronizedImages(IEnumerable<IImageBox> affectedImageBoxes)
		{
			List<IImageBox> imageBoxesToDraw = new List<IImageBox>();

			if (_stackingSynchronizationTool != null)
				imageBoxesToDraw.InsertRange(0, affectedImageBoxes);

			List<IPresentationImage> affectedImages = new List<IPresentationImage>();

			if (_referenceLineTool != null)
			{
				foreach (IPresentationImage image in _referenceLineTool.CalculateReferenceLines())
				{
					if (!imageBoxesToDraw.Contains(image.ParentDisplaySet.ImageBox))
						affectedImages.Add(image);
				}
			}

			foreach (IImageBox imageBox in imageBoxesToDraw)
				imageBox.Draw();

			OnReferenceLinesCalculated(affectedImages);
		}

		private void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			if (e.SelectedTile.PresentationImage != null)
			{
				//the presentation image selected event will fire and take care of this.
				return;
			}

			if (_referenceLineTool != null)
			{
				OnReferenceLinesCalculated(_referenceLineTool.CalculateReferenceLines());
			}
		}

		private void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			if (_stackingSynchronizationTool != null)
			{
				OnSynchronizedImages(_stackingSynchronizationTool.Synchronize());
			}
		}
	}
}
