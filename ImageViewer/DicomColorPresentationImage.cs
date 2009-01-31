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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM colour <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomColorPresentationImage
		: ColorPresentationImage, IImageSopProvider
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating an
		/// <see cref="Frame"/> with a <see cref="ColorPresentationImage"/>.
		/// </remarks>
		public DicomColorPresentationImage(Frame frame)
			: this(frame.CreateTransientReference())
		{
			Platform.CheckForNullReference(frame, "frame");

			_frameReference = frame.CreateTransientReference();
		}

		public DicomColorPresentationImage(IFrameReference frameReference)
			: base(frameReference.Frame.Rows,
				   frameReference.Frame.Columns,
				   frameReference.Frame.NormalizedPixelSpacing.Column,
				   frameReference.Frame.NormalizedPixelSpacing.Row,
				   frameReference.Frame.PixelAspectRatio.Column,
				   frameReference.Frame.PixelAspectRatio.Row,
				   frameReference.Frame.GetNormalizedPixelData)
		{
			_frameReference = frameReference;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomColorPresentationImage(DicomColorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomColorPresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomColorPresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			return new DicomColorPresentationImage(Frame);
		}

		#region IImageSopProvider members

		/// <summary>
		/// Gets this presentation image's associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// Use <see cref="ImageSop"/> to access DICOM tags.
		/// </remarks>
		public ImageSop ImageSop
		{
			get { return Frame.ParentImageSop; }
		}

		/// <summary>
		/// Gets this presentation image's associated <see cref="Frame"/>.
		/// </summary>
		public Frame Frame
		{
			get { return _frameReference.Frame; }
		}

		#endregion

		/// <summary>
		/// Dispose method.  Inheritors should override this method to do any additional cleanup.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && _frameReference != null)
			{
				_frameReference.Dispose();
				_frameReference = null;
			}
			
			base.Dispose(disposing);
		}

		/// <summary>
		/// Creates the <see cref="IAnnotationLayout"/> for this image.
		/// </summary>
		/// <returns></returns>
		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			return DicomAnnotationLayoutFactory.CreateLayout(this);
		}

		/// <summary>
		/// Returns the Instance Number as a string.
		/// </summary>
		/// <returns>The Instance Number as a string.</returns>
		public override string ToString()
		{
			return Frame.ParentImageSop.InstanceNumber.ToString();
		}
	}
}
