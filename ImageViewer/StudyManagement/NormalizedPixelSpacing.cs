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
		private Frame _frame;

		/// <summary>
		/// Initializes a new instance of <see cref="NormalizedPixelSpacing"/>.
		/// </summary>
		internal NormalizedPixelSpacing(Frame frame)
		{
			_frame = frame;
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
			if (String.Compare(_frame.ParentImageSop.Modality, "CR", true) == 0 ||
			    String.Compare(_frame.ParentImageSop.Modality, "DX", true) == 0 ||
			    String.Compare(_frame.ParentImageSop.Modality, "MG", true) == 0)
			{
				double pixelSpacingRow = 0, pixelSpacingColumn = 0;

				bool tagExists;
				_frame.ParentImageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingRow, 0, out tagExists);

				if (tagExists)
					_frame.ParentImageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingColumn, 1, out tagExists);

				if (!tagExists)
					pixelSpacingRow = pixelSpacingColumn = 0;

				this.Row = pixelSpacingRow;
				this.Column = pixelSpacingColumn;
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
