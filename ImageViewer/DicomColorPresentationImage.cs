#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.DicomGraphics;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM colour <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomColorPresentationImage : ColorPresentationImage, IDicomPresentationImage
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		[CloneIgnore]
		private CompositeGraphic _dicomGraphics;

		[CloneIgnore]
		private DicomGraphicsDeserializer _graphicsDeserializer;

		private bool _presentationStateApplied = false;
		[CloneCopyReference]
		private DicomSoftcopyPresentationState _presentationState;

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
			_graphicsDeserializer = new DicomGraphicsDeserializer(this);
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomColorPresentationImage(DicomColorPresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();

			if (source._graphicsDeserializer != null)
				_graphicsDeserializer = new DicomGraphicsDeserializer(this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_dicomGraphics = CollectionUtils.SelectFirst(base.GraphicalLayers,
				delegate(IGraphic test) { return test.Name == "DICOM"; }) as CompositeGraphic;

			Initialize();
		}

		private void Initialize()
		{
			if (_dicomGraphics == null)
			{
				_dicomGraphics = new CompositeGraphic();
				_dicomGraphics.Name = "DICOM";

				// insert the DICOM graphics layer right after the image graphic (both contain domain-level graphics)
				IGraphic imageGraphic = CollectionUtils.SelectFirst(base.GraphicalLayers,
					delegate(IGraphic test) { return test is ImageGraphic; });
				base.GraphicalLayers.Insert(base.GraphicalLayers.IndexOf(imageGraphic) + 1, _dicomGraphics);
			}
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
			DicomColorPresentationImage image = new DicomColorPresentationImage(Frame);
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

		public DicomSoftcopyPresentationState PresentationState
		{
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

		#region IDicomPresentationImage Members

		public GraphicCollection DicomGraphics
		{
			get { return _dicomGraphics.Graphics; }
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
		protected override void OnDrawing()
		{
			bool anyFailures = false;

			try
			{
				if (_graphicsDeserializer != null)
					_graphicsDeserializer.Deserialize();
			}
			catch (Exception e)
			{
				anyFailures = true;
				Platform.Log(LogLevel.Warn, e, "An error has occurred while deserializing graphics from the image header.");
			}
			finally
			{
				_graphicsDeserializer = null; //it's been done and any clones of this image will have their graphics cloned
			}

			if (!_presentationStateApplied && this.PresentationState != null)
			{
				_presentationStateApplied = true;

				try
				{
					this.PresentationState.Deserialize(this);
				}
				catch (Exception ex)
				{
					anyFailures = true;
					Platform.Log(LogLevel.Warn, ex, "An error has occurred while deserializing the image presentation state.");
				}
			}

			if (anyFailures)
				throw new Exception("At least one failure has occurred while deserializing the image presentation state.");

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
