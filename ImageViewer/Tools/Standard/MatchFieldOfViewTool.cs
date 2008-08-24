using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
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

			ImageSpatialTransform transform = GetImageTransform(image);
			if (transform == null)
				return false;

			Frame frame = GetFrame(image);
			if (frame == null || frame.NormalizedPixelSpacing.IsNull)
				return false;

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			ImageSpatialTransform matchTransform = GetImageTransform(image);

			// this is the reference image box, so we just want to turn off 'scale to fit'
			// and set the scale to be the same as the reference image.
			if (image.ParentDisplaySet.ImageBox == ReferenceImageBox)
			{
				ImageSpatialTransform referenceTransform = GetImageTransform(ReferenceImage);
				matchTransform.ScaleToFit = false;
				matchTransform.Scale = referenceTransform.Scale;
				return;
			}

			//match * x = reference
			float referenceDisplayedWidth = GetDisplayedWidth(ReferenceImage, ReferenceImage.ClientRectangle);
			float matchDisplayedWidth = GetDisplayedWidth(image, image.ParentDisplaySet.ImageBox.Tiles[0].ClientRectangle);

			matchTransform.ScaleToFit = false;
			matchTransform.Scale *= matchDisplayedWidth / referenceDisplayedWidth;
		}

		#endregion

		#region Private Methods

		private static float GetDisplayedWidth(IPresentationImage presentationImage, Rectangle clientRectangle)
		{
			ImageSpatialTransform transform = GetImageTransform(presentationImage);
			Frame frame = GetFrame(presentationImage);

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			RectangleF destinationRectangle = transform.ConvertToDestination(sourceRectangle);
			destinationRectangle = RectangleUtilities.Intersect(destinationRectangle, clientRectangle);

			float effectivePixelSizeX = (float)frame.NormalizedPixelSpacing.Column / transform.Scale;
			float effectivePixelSizeY = (float)frame.NormalizedPixelSpacing.Row / transform.Scale;

			// DFOV in cm
			if (!IsRotated(transform))
				return Math.Abs(destinationRectangle.Width * effectivePixelSizeX / 10);
			else
				return Math.Abs(destinationRectangle.Width * effectivePixelSizeY / 10);
		}

		private static bool IsRotated(SpatialTransform transform)
		{
			float m12 = transform.CumulativeTransform.Elements[2];
			return !FloatComparer.AreEqual(m12, 0.0f, 0.001f);
		}

		private static ImageSpatialTransform GetImageTransform(IPresentationImage image)
		{
			if (image != null && image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform as ImageSpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image != null && image is IImageSopProvider)
				return ((IImageSopProvider)image).Frame;

			return null;
		}

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

		#endregion
	}
}
