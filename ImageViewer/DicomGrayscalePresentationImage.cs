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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM grayscale <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomGrayscalePresentationImage 
		: GrayscalePresentationImage, IImageSopProvider, IDicomSoftcopyPresentationStateProvider
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		private bool _presentationStateApplied = false;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating a
		/// <see cref="Frame"/> with a <see cref="GrayscalePresentationImage"/>.
		/// </remarks>
		public DicomGrayscalePresentationImage(Frame frame)
			: this(frame.CreateTransientReference())
		{
			Platform.CheckForNullReference(frame, "frame");
			_frameReference = frame.CreateTransientReference();
		}
		
		public DicomGrayscalePresentationImage(IFrameReference frameReference)
			: base(frameReference.Frame.Rows,
				   frameReference.Frame.Columns,
				   frameReference.Frame.BitsAllocated,
				   frameReference.Frame.BitsStored,
				   frameReference.Frame.HighBit,
				   frameReference.Frame.PixelRepresentation != 0 ? true : false,
				   frameReference.Frame.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ? true : false,
				   frameReference.Frame.RescaleSlope,
				   frameReference.Frame.RescaleIntercept,
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
		protected DicomGrayscalePresentationImage(DicomGrayscalePresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();
		}

		/// <summary>
		/// Creates a fresh copy of the <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <remarks>
		/// This will instantiate a fresh copy of this <see cref="DicomGrayscalePresentationImage"/>
		/// using the same construction parameters as the original.
		/// </remarks>
		/// <returns></returns>		
		public override IPresentationImage CreateFreshCopy()
		{
			DicomGrayscalePresentationImage image = new DicomGrayscalePresentationImage(Frame);
			image.PresentationState = this.PresentationState;
			return image;
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

		#region IDicomSoftcopyPresentationStateProvider Members

		[CloneCopyReference]
		private DicomSoftcopyPresentationState _presentationState;

		public DicomSoftcopyPresentationState PresentationState {
			get { return _presentationState; }
			set
			{
				if (_presentationState != value)
				{
					_presentationState = value;
					_presentationStateApplied = false;
				}
			}
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
		/// Raises the <see cref="PresentationImage.Drawing"/> event.
		/// </summary>
		protected override void OnDrawing() {
			if(!_presentationStateApplied && this.PresentationState != null)
			{
				_presentationStateApplied = true;
				this.PresentationState.Apply(this);
			}

			base.OnDrawing();
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
