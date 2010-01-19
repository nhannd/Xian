using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Layout.Basic
{
	/// <summary>
	/// A <see cref="DicomPresentationState"/> class that applies very simple transformations to
	/// the default presentation state of a DICOM-based <see cref="PresentationImage"/>.
	/// </summary>
	/// <remarks>Eventually, this will be expanded to support all of the Presentation
	/// Intent attributes from DICOM Supplement 60, but right now it only contains the
	/// "Show Grayscale Inverted" attribute.</remarks>
	[Cloneable(true)]
	internal class BasicDicomPresentationState : PresentationState
	{
		/// <summary>
		/// Initializes an instance of <see cref="BasicDicomPresentationState"/>.
		/// </summary>
		public BasicDicomPresentationState()
		{}

		public bool ShowGrayscaleInverted { get; set; }

		/// <summary>
		/// Not supported.
		/// </summary>
		/// <exception cref="NotSupportedException">Thrown always.</exception>
		public override void Serialize(IEnumerable<IPresentationImage> images)
		{
			throw new System.NotSupportedException("Simple presentation state objects cannot be serialized.");
		}

		/// <summary>
		/// Deserializes the presentation state, applying the changes to each image
		/// in <paramref name="images"/>.
		/// </summary>
		/// <remarks>
		/// The presentation state applied by <see cref="DicomSoftcopyPresentationState.Default"/> is
		/// deserialized first, followed by any specializations of this instance.
		/// </remarks>
		public override void Deserialize(IEnumerable<IPresentationImage> images)
		{
			DicomDefault.Deserialize(images);

			if (ShowGrayscaleInverted)
			{
				foreach (IPresentationImage image in images)
				{
					if (image is IImageGraphicProvider)
					{
						if (!(((IImageGraphicProvider)image).ImageGraphic.PixelData is GrayscalePixelData))
							continue;
					}

					if (!(image is IVoiLutProvider))
						continue;

					IVoiLutManager manager = ((IVoiLutProvider)image).VoiLutManager;
					//Invert can be true by default depending on photometric interpretation,
					//so we'll just assume the current value is the default and flip it.
					manager.Invert = !manager.Invert;
				}
			}
		}

		public override void Clear(IEnumerable<IPresentationImage> image)
		{
		}
	}
}