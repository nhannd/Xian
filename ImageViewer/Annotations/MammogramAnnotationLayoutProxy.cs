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
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Annotations
{
	[Cloneable(true)]
	internal class MammogramAnnotationLayoutProxy : IAnnotationLayout
	{
		[CloneIgnore]
		private DicomGrayscalePresentationImage _ownerImage;

		private bool _visible = true;

		public DicomGrayscalePresentationImage OwnerImage
		{
			get { return _ownerImage; }
			set { _ownerImage = value; }
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				if (_ownerImage != null && _ownerImage.SpatialTransform is MammographyImageSpatialTransform)
				{
					string effectiveRowOrientation, effectiveColumnOrientation;
					((MammographyImageSpatialTransform) _ownerImage.SpatialTransform).GetEffectivePosteriorPatientOrientation(out effectiveRowOrientation, out effectiveColumnOrientation);
					var filterCandidates = new List<KeyValuePair<string, string>>();
					filterCandidates.Add(new KeyValuePair<string, string>("Modality", _ownerImage.ImageSop.Modality));
					filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Row", effectiveRowOrientation));
					filterCandidates.Add(new KeyValuePair<string, string>("PatientOrientation_Col", effectiveColumnOrientation));
					return DicomAnnotationLayoutFactory.CreateLayout(filterCandidates).AnnotationBoxes;
				}
				return DicomAnnotationLayoutFactory.CreateLayout(_ownerImage).AnnotationBoxes;
			}
		}

		public IAnnotationLayout Clone()
		{
			return new MammogramAnnotationLayoutProxy();
		}
	}
}