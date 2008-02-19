using System;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// The pixel spacing appropriate to the modality.
	/// </summary>
	/// <remarks>
	/// For projection based modalities (i.e. CR, DX and MG), Imager Pixel Spacing is
	/// returned as the pixel spacing.  For all other modalities, the standard
	/// Pixel Spacing is returned.
	/// </remarks>
	public class NormalizedPixelSpacing : PixelSpacing
	{
		private ImageSop _imageSop;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizedPixelSpacing"/>.
		/// </summary>
		internal NormalizedPixelSpacing(ImageSop imageSop)
		{
			_imageSop = imageSop;
			Initialize();
		}

		/// <summary>
		/// Manually calibrates the pixel spacing.
		/// </summary>
		/// <param name="pixelSpacingRow">Pixel height.</param>
		/// <param name="pixelSpacingColumn">Pixel width.</param>
		/// <remarks>
		/// Using this method does not alter the original DICOM pixel spacing
		/// contained in <see cref="ImageSop.PixelSpacing"/>.
		/// </remarks>
		public void Calibrate(double pixelSpacingRow, double pixelSpacingColumn)
		{
			this.Row = pixelSpacingRow;
			this.Column = pixelSpacingColumn;
		}

		private void Initialize()
		{
			// If CR, CX or MG, use ImagePixelSpacing instead
			if (String.Compare(_imageSop.Modality, "CR", true) == 0 ||
			    String.Compare(_imageSop.Modality, "DX", true) == 0 ||
			    String.Compare(_imageSop.Modality, "MG", true) == 0)
			{
				double pixelSpacingRow = 0, pixelSpacingColumn = 0;

				bool tagExists;
				_imageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingRow, 0, out tagExists);

				if (tagExists)
					_imageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingColumn, 1, out tagExists);

				if (!tagExists)
					pixelSpacingRow = pixelSpacingColumn = 0;

				this.Row = pixelSpacingRow;
				this.Column = pixelSpacingColumn;
			}
			// Otherwise, just use pixel spacing
			else
			{
				this.Row = _imageSop.PixelSpacing.Row;
				this.Column = _imageSop.PixelSpacing.Column;
			}

			Validate();
		}

		private void Validate()
		{
			if (this.Row <= float.Epsilon ||
			    this.Column <= float.Epsilon ||
			    double.IsNaN(this.Row) ||
			    double.IsNaN(this.Column))
			{
				this.Row = 0;
				this.Column = 0;
			}
		}
	}
}
