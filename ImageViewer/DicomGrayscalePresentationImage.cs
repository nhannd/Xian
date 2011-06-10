#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A DICOM grayscale <see cref="PresentationImage"/>.
	/// </summary>
	[Cloneable]
	public class DicomGrayscalePresentationImage : GrayscalePresentationImage, IDicomPresentationImage, IDicomVoiLutsProvider
	{
		[CloneIgnore]
		private IFrameReference _frameReference;

		[CloneIgnore]
		private CompositeGraphic _dicomGraphics;

		[CloneIgnore]
		private readonly DicomVoiLuts _dicomVoiLuts;

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frame">The <see cref="Frame"/> from which to construct the image.</param>
		/// <remarks>
		/// This constructor provides a convenient means of associating a <see cref="Frame"/> with a <see cref="GrayscalePresentationImage"/>.
		/// </remarks>
		public DicomGrayscalePresentationImage(Frame frame)
			: this(frame.CreateTransientReference())
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomGrayscalePresentationImage"/>.
		/// </summary>
		/// <param name="frameReference">A <see cref="IFrameReference">reference</see> to the frame from which to construct the image.</param>
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
			_dicomVoiLuts = new DicomVoiLuts(this);
			base.PresentationState = PresentationState.DicomDefault;

			if (ImageSop.Modality == "MG")
			{
				// use a special image spatial transform for digital mammography
				CompositeImageGraphic.SpatialTransform = new MammographyImageSpatialTransform(CompositeImageGraphic, Frame.Rows, Frame.Columns, Frame.NormalizedPixelSpacing.Column, Frame.NormalizedPixelSpacing.Row, Frame.PixelAspectRatio.Column, Frame.PixelAspectRatio.Row, Frame.PatientOrientation, ImageSop.ImageLaterality);
			}

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected DicomGrayscalePresentationImage(DicomGrayscalePresentationImage source, ICloningContext context)
			: base(source, context)
		{
			Frame frame = source.Frame;
			_frameReference = frame.CreateTransientReference();
			_dicomVoiLuts = new DicomVoiLuts(this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_dicomGraphics = CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics,
				delegate(IGraphic test) { return test.Name == "DICOM"; }) as CompositeGraphic;

			Initialize();
		}

		private void Initialize()
		{
			if (base.ImageGraphic.VoiLutFactory == null)
			{
				base.ImageGraphic.VoiLutFactory = GraphicVoiLutFactory.Create(GetInitialVoiLut);
			}

			if (_dicomGraphics == null)
			{
				_dicomGraphics = new CompositeGraphic();
				_dicomGraphics.Name = "DICOM";

				// insert the DICOM graphics layer right after the image graphic (both contain domain-level graphics)
				IGraphic imageGraphic = CollectionUtils.SelectFirst(base.CompositeImageGraphic.Graphics, g => g is ImageGraphic);
				base.CompositeImageGraphic.Graphics.Insert(base.CompositeImageGraphic.Graphics.IndexOf(imageGraphic) + 1, _dicomGraphics);
			}
		}

		private static IComposableLut GetInitialVoiLut(IGraphic graphic)
		{
			GrayscaleImageGraphic grayImageGraphic = (GrayscaleImageGraphic) graphic;
			IComposableLut lut = InitialVoiLutProvider.Instance.GetLut(graphic.ParentPresentationImage);
			if (lut == null)
				lut = new MinMaxPixelCalculatedLinearLut(grayImageGraphic.PixelData, grayImageGraphic.ModalityLut);
			return lut;
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

		#region ISopProvider Members

		Sop ISopProvider.Sop
		{
			get { return _frameReference.Sop; }
		}

		#endregion

		#region IDicomVoiLutsProvider Members

		/// <summary>
		/// Gets a collection of DICOM-defined VOI LUTs from the image header and/or any associated presentation state.
		/// </summary>
		public IDicomVoiLuts DicomVoiLuts
		{
			get { return _dicomVoiLuts; }
		}

		#endregion

		#region IDicomPresentationImage Members

		/// <summary>
		/// Gets this presentation image's collection of domain-level graphics.
		/// </summary>
		/// <remarks>
		/// Use <see cref="IDicomPresentationImage.DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </remarks>
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
		/// Creates the <see cref="IAnnotationLayout"/> for this image.
		/// </summary>
		/// <returns></returns>
		protected override IAnnotationLayout CreateAnnotationLayout()
		{
			return DicomAnnotationLayoutFactory.CreateLayout(this);
		}

		protected override void OnDrawing()
		{
			base.OnDrawing();

			if (SpatialTransform is MammographyImageSpatialTransform)
			{
				string effectiveRowOrientation, effectiveColumnOrientation;
				((MammographyImageSpatialTransform) SpatialTransform).GetEffectivePosteriorPatientOrientation(out effectiveRowOrientation, out effectiveColumnOrientation);
				var filterCandidates = new List<KeyValuePair<string, string>>();
				filterCandidates.Add(new KeyValuePair<string, string>("Modality", ImageSop.Modality));
				filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Row", effectiveRowOrientation));
				filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Col", effectiveColumnOrientation));
				AnnotationLayout = DicomAnnotationLayoutFactory.CreateLayout(filterCandidates);
			}
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
