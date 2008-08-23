using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuMatchFieldOfView", "MatchFieldOfView")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarMatchFieldOfView", "MatchFieldOfView")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardMatchFieldOfView/Activate", "MatchFieldOfView", KeyStroke = XKeys.F)]
	[IconSet("activate", IconScheme.Colour, "Icons.MatchFieldOfViewToolSmall.png", "Icons.MatchFieldOfViewToolMedium.png", "Icons.MatchFieldOfViewToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class MatchFieldOfViewTool : ImageViewerTool, IUndoableOperation<IPresentationImage>
	{
		public MatchFieldOfViewTool()
		{
		}

		private IImageBox ReferenceImageBox
		{
			get { return base.ImageViewer.SelectedImageBox; }
		}

		private IPresentationImage ReferenceImage
		{
			get { return base.ImageViewer.SelectedPresentationImage; }
		}

		public void MatchFieldOfView()
		{
			if (!AppliesTo(ReferenceImage))
				return;

			UndoableOperationApplicator<IPresentationImage> applicator = 
				new UndoableOperationApplicator<IPresentationImage>(this, GetAllImages());

			applicator.ItemMementoSet += RedrawImage;

			UndoableCommand command = new UndoableCommand(applicator);
			command.Name = SR.CommandMatchFieldOfView;
			command.BeginState = applicator.CreateMemento();

			applicator.Apply();

			command.EndState = applicator.CreateMemento();
			if (!command.BeginState.Equals(command.EndState))
				base.ImageViewer.CommandHistory.AddCommand(command);

			base.ImageViewer.PhysicalWorkspace.Draw();
		}

		#region IUndoableOperation<IPresentationImage> Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			return GetImageTransform(image);
		}

		public bool AppliesTo(IPresentationImage image)
		{
			if (image == null)
				return false;

			IImageSpatialTransform transform = GetImageTransform(image);
			if (transform == null)
				return false;

			Frame frame = GetFrame(image);
			if (frame == null || frame.NormalizedPixelSpacing.IsNull)
				return false;

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			IImageSpatialTransform referenceTransform = GetImageTransform(ReferenceImage);
			IImageSpatialTransform matchTransform = GetImageTransform(image);

			// this is the reference image box, so we just want to turn off 'scale to fit'
			// and set the scale to be the same as the reference image.
			if (image.ParentDisplaySet.ImageBox == ReferenceImageBox)
			{
				matchTransform.ScaleToFit = false;
				matchTransform.Scale = referenceTransform.Scale;
				return;
			}

			Frame referenceFrame = GetFrame(ReferenceImage);
			Frame matchFrame = GetFrame(image);

			//effective size in mm/pixel
			float effectivePixelSize = (float)referenceFrame.NormalizedPixelSpacing.Column / referenceTransform.Scale;

			matchTransform.ScaleToFit = false;
			matchTransform.Scale = (float)matchFrame.NormalizedPixelSpacing.Column / effectivePixelSize;
		}

		#endregion

		private static void RedrawImage(object sender, ItemEventArgs<IPresentationImage> e)
		{
			e.Item.Draw();
		}

		private IEnumerable<IPresentationImage> GetAllImages()
		{
			foreach (IImageBox imageBox in base.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet == null)
					continue;
				
				foreach (IPresentationImage image in imageBox.DisplaySet.PresentationImages)
					yield return image;
			}
		}

		private static IImageSpatialTransform GetImageTransform(IPresentationImage image)
		{
			if (image != null && image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform as IImageSpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image != null && image is IImageSopProvider)
				return ((IImageSopProvider)image).Frame;

			return null;
		}
	}
}
