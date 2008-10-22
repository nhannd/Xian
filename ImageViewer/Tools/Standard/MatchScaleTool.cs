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
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuMatchScale", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarMatchScale", "Activate")]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardMatchScaleTool/Activate", "Activate", KeyStroke = XKeys.M)]
	[IconSet("activate", IconScheme.Colour, "Icons.MatchScaleToolSmall.png", "Icons.MatchScaleToolMedium.png", "Icons.MatchScaleToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class MatchScaleTool : ImageViewerTool, IUndoableOperation<IPresentationImage>
	{
		#region Private Fields

		private float _referenceDisplayedWidth;
		private RectangleF _referenceDisplayRectangle;

		#endregion

		public MatchScaleTool()
		{
		}

		#region Private Properties

		private IImageBox ReferenceImageBox
		{
			get { return base.ImageViewer.SelectedImageBox; }
		}

		private IPresentationImage ReferenceImage
		{
			get { return base.ImageViewer.SelectedPresentationImage; }
		}

		#endregion

		#region Public Methods

		public void Activate()
		{
			if (!AppliesTo(ReferenceImage))
				return;

			UndoableOperationApplicator<IPresentationImage> applicator = 
				new UndoableOperationApplicator<IPresentationImage>(this, GetAllImages());

			applicator.AppliedOperation += RedrawImage;
			applicator.ItemMementoSet += RedrawImage;

			MemorableUndoableCommand command = new MemorableUndoableCommand(applicator);
			command.Name = SR.CommandMatchScale;
			command.BeginState = applicator.CreateMemento();

			CalculateReferenceDisplayValues();
			applicator.Apply();

			command.EndState = applicator.CreateMemento();
			if (!command.BeginState.Equals(command.EndState))
				base.ImageViewer.CommandHistory.AddCommand(command);
		}

		#endregion

		#region IUndoableOperation<IPresentationImage> Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			return GetImageTransform(image);
		}

		public bool AppliesTo(IPresentationImage image)
		{
			ImageSpatialTransform transform = GetImageTransform(image);
			if (transform == null)
				return false;

			//mustn't be rotated at a non-right angle to the viewport.
			if (transform.RotationXY % 90 != 0)
				return false;

			Frame frame = GetFrame(image);
			if (frame == null || frame.NormalizedPixelSpacing.IsNull)
				return false;

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			ImageSpatialTransform matchTransform = GetImageTransform(image);

			if (image.ParentDisplaySet.ImageBox == ReferenceImageBox)
			{
				// this is the reference image box, so we just want to turn off 'scale to fit'
				// and set the scale to be the same as the reference image.
				ImageSpatialTransform referenceTransform = GetImageTransform(ReferenceImage);
				matchTransform.ScaleToFit = false;
				matchTransform.Scale = referenceTransform.Scale;
			}
			else
			{
				//get the displayed width (in mm) for the same size display rectangle in the image to be matched.
				float matchDisplayedWidth = GetDisplayedWidth(image, _referenceDisplayRectangle);

				float rescaleAmount = matchDisplayedWidth/_referenceDisplayedWidth;
				matchTransform.ScaleToFit = false;

				if (FloatComparer.AreEqual(rescaleAmount, 1.0F))
					return;

				matchTransform.Scale *= rescaleAmount;
			}
		}

		#endregion

		#region Private Methods

		private void CalculateReferenceDisplayValues()
		{
			ImageSpatialTransform transform = GetImageTransform(ReferenceImage);
			Frame frame = GetFrame(ReferenceImage);

			//calculate the width (in mm) of the portion of the image that is visible on the display,
			//as well as the display rectangle it occupies.

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			_referenceDisplayRectangle = transform.ConvertToDestination(sourceRectangle);
			_referenceDisplayRectangle = RectangleUtilities.Intersect(_referenceDisplayRectangle, ReferenceImage.ClientRectangle);

			_referenceDisplayedWidth = GetDisplayedWidth(ReferenceImage, _referenceDisplayRectangle);
		}

		#region Private Helper Methods

		private static float GetDisplayedWidth(IPresentationImage presentationImage, RectangleF referenceDisplayedRectangle)
		{
			ImageSpatialTransform transform = GetImageTransform(presentationImage);
			Frame frame = GetFrame(presentationImage);

			float effectivePixelSizeX = (float)frame.NormalizedPixelSpacing.Column / transform.Scale;
			float effectivePixelSizeY = (float)frame.NormalizedPixelSpacing.Row / transform.Scale;

			if (transform.RotationXY == 90 || transform.RotationXY == 270)
				return Math.Abs(referenceDisplayedRectangle.Width * effectivePixelSizeY / 10);
			else
				return Math.Abs(referenceDisplayedRectangle.Width * effectivePixelSizeX / 10);
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

		private static void RedrawImage(object sender, ItemEventArgs<IPresentationImage> e)
		{
			e.Item.Draw();
		}

		#endregion
		#endregion
	}
}
