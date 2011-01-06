#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO (cr Feb 2010): There are no unit tests for this, but our confidence level is high
	//based on the proven accuracy of the sync tools and MPR, which are all based on this.

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
		private event EventHandler _calibrated;
		private readonly Frame _frame;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizedPixelSpacing"/>.
		/// </summary>
		internal NormalizedPixelSpacing(Frame frame)
		{
			_frame = frame;
			Initialize();
		}

		/// <summary>
		/// Event fired when the pixel spacing is calibrated.
		/// </summary>
		public event EventHandler Calibrated
		{
			add { _calibrated += value; }
			remove { _calibrated -= value; }
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
			EventsHelper.Fire(_calibrated, this, new EventArgs());
		}

		private void Initialize()
		{
			// If CR, CX or MG, use ImagePixelSpacing instead
			if (String.Compare(_frame.ParentImageSop.Modality, "CR", true) == 0 ||
			    String.Compare(_frame.ParentImageSop.Modality, "DX", true) == 0 ||
			    String.Compare(_frame.ParentImageSop.Modality, "MG", true) == 0)
			{
				this.Row = _frame.ImagerPixelSpacing.Row;
				this.Column = _frame.ImagerPixelSpacing.Column;
			}
			// Otherwise, just use pixel spacing
			else
			{
				this.Row = _frame.PixelSpacing.Row;
				this.Column = _frame.PixelSpacing.Column;
			}
		}
	}
}
