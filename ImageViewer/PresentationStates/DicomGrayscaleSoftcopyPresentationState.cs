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

using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[Cloneable]
	internal sealed class DicomGrayscaleSoftcopyPresentationState : DicomSoftcopyPresentationStateMaskLutBase<DicomGrayscalePresentationImage>
	{
		public static readonly SopClass SopClass = SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass;

		public DicomGrayscaleSoftcopyPresentationState() : base(SopClass) {}

		public DicomGrayscaleSoftcopyPresentationState(DicomFile dicomFile) : base(SopClass, dicomFile) {}

		public DicomGrayscaleSoftcopyPresentationState(DicomAttributeCollection dataSource) : base(SopClass, dataSource) {}

		public DicomGrayscaleSoftcopyPresentationState(IDicomAttributeProvider dataSource) : base(SopClass, ShallowCopyDataSource(dataSource)) { }

		private DicomGrayscaleSoftcopyPresentationState(DicomGrayscaleSoftcopyPresentationState source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		#region Serialization Support

		protected override void PerformTypeSpecificSerialization(IList<DicomGrayscalePresentationImage> imagesByList, IDictionary<string, IList<DicomGrayscalePresentationImage>> imagesBySeries)
		{
			GrayscaleSoftcopyPresentationStateIod iod = new GrayscaleSoftcopyPresentationStateIod(base.DataSet);
			this.SerializePresentationStateRelationship(iod.PresentationStateRelationship, imagesBySeries);
			this.SerializePresentationStateShutter(iod.PresentationStateShutter);
			this.SerializePresentationStateMask(iod.PresentationStateMask);
			this.SerializeMask(iod.Mask);
			this.SerializeDisplayShutter(iod.DisplayShutter);
			this.SerializeBitmapDisplayShutter(iod.BitmapDisplayShutter);
			this.SerializeOverlayPlane(iod.OverlayPlane);
			this.SerializeOverlayActivation(iod.OverlayActivation);
			this.SerializeDisplayedArea(iod.DisplayedArea, imagesByList);
			this.SerializeGraphicAnnotation(iod.GraphicAnnotation, imagesByList);
			this.SerializeSpatialTransform(iod.SpatialTransform, imagesByList);
			this.SerializeGraphicLayer(iod.GraphicLayer, imagesByList);
			this.SerializeModalityLut(iod.ModalityLut);
			this.SerializeSoftcopyVoiLut(iod.SoftcopyVoiLut, imagesByList);
			this.SerializeSoftcopyPresentationLut(iod.SoftcopyPresentationLut);
		}

		/// <summary>
		/// Serializes the Softcopy Presentation LUT IOD module (DICOM PS 3.3, C.11.6)
		/// </summary>
		/// <remarks>Softcopy Presentation LUTs are not currently supported by this product.</remarks>
		/// <param name="module">The IOD module.</param>
		private void SerializeSoftcopyPresentationLut(SoftcopyPresentationLutModuleIod module)
		{
			module.InitializeAttributes();
			module.PresentationLutShape = PresentationLutShape.Identity;
		}

		#endregion

		#region Deserialization Support

		protected override void PerformTypeSpecificDeserialization(IList<DicomGrayscalePresentationImage> imagesByList, IDictionary<string, IList<DicomGrayscalePresentationImage>> imagesBySeries)
		{
			GrayscaleSoftcopyPresentationStateIod iod = new GrayscaleSoftcopyPresentationStateIod(base.DataSet);

			foreach (DicomGrayscalePresentationImage image in imagesByList)
			{
				RectangleF displayedArea;
				this.DeserializeSpatialTransform(iod.SpatialTransform, image);
				this.DeserializeDisplayedArea(iod.DisplayedArea, out displayedArea, image);
				this.DeserializeGraphicLayer(iod.GraphicLayer, image);
				this.DeserializeGraphicAnnotation(iod.GraphicAnnotation, displayedArea, image);
				this.DeserializeModalityLut(iod.ModalityLut, image);
				this.DeserializeSoftcopyVoiLut(iod.SoftcopyVoiLut, image);
				this.DeserializeSoftcopyPresentationLut(iod.SoftcopyPresentationLut, image);
				image.Draw();
			}
		}

		/// <summary>
		/// Deserializes the Softcopy Presentation LUT IOD module (DICOM PS 3.3, C.11.6)
		/// </summary>
		/// <remarks>Softcopy Presentation LUTs are not currently supported by this product.</remarks>
		/// <param name="module">The IOD module.</param>
		/// <param name="image">The <see cref="IPresentationImage"/> to deserialize to.</param>
		private void DeserializeSoftcopyPresentationLut(SoftcopyPresentationLutModuleIod module, DicomGrayscalePresentationImage image) {}

		#endregion

		#region IDicomAttributeProvider Copy Method

		private static DicomAttributeCollection ShallowCopyDataSource(IDicomAttributeProvider source)
		{
			if(source is DicomAttributeCollection)
				return (DicomAttributeCollection) source;

			// a shallow copy is sufficient - even if the provider is a sop object that can be user-disposed, it
			// provides an indexer to get dicom attribute objects which will not be disposed if we have a reference to it
			DicomAttributeCollection collection = new DicomAttributeCollection();

			foreach (uint tag in PatientModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialSubjectModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PatientStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralEquipmentModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateIdentificationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateRelationshipModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateMaskModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in MaskModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in DisplayShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in BitmapDisplayShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in OverlayPlaneModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in OverlayActivationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in DisplayedAreaModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GraphicAnnotationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SpatialTransformModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GraphicLayerModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ModalityLutModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SoftcopyVoiLutModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SoftcopyPresentationLutModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SopCommonModuleIod.DefinedTags)
				collection[tag] = source[tag];

			return collection;
		}

		#endregion
	}
}
