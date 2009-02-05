using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates {
	[Cloneable]
	internal sealed class DicomColorSoftcopyPresentationState : DicomSoftcopyPresentationStateBase<DicomColorPresentationImage> {
		public static readonly SopClass SopClass = SopClass.ColorSoftcopyPresentationStateStorageSopClass;

		public DicomColorSoftcopyPresentationState() : base(SopClass) { }

		public DicomColorSoftcopyPresentationState(DicomFile dicomFile) : base(SopClass, dicomFile) { }

		public DicomColorSoftcopyPresentationState(DicomAttributeCollection dataSource) : base(SopClass, dataSource) { }

		public DicomColorSoftcopyPresentationState(IDicomAttributeProvider dataSource) : base(SopClass, ShallowCopyDataSource(dataSource)) { }

		private DicomColorSoftcopyPresentationState(DicomColorSoftcopyPresentationState source, ICloningContext context)
			: base(source, context) {
			context.CloneFields(source, this);
		}

		#region Serialization Support

		protected override void PerformTypeSpecificSerialization(IList<DicomColorPresentationImage> imagesByList, IDictionary<string, IList<DicomColorPresentationImage>> imagesBySeries) {
			ColorSoftcopyPresentationStateIod iod = new ColorSoftcopyPresentationStateIod(base.DataSet);
			this.SerializePresentationStateRelationship(iod.PresentationStateRelationship, imagesBySeries);
			this.SerializePresentationStateShutter(iod.PresentationStateShutter);
			this.SerializeDisplayShutter(iod.DisplayShutter);
			this.SerializeBitmapDisplayShutter(iod.BitmapDisplayShutter);
			this.SerializeOverlayPlane(iod.OverlayPlane);
			this.SerializeOverlayActivation(iod.OverlayActivation);
			this.SerializeDisplayedArea(iod.DisplayedArea, imagesByList);
			this.SerializeGraphicAnnotation(iod.GraphicAnnotation, imagesByList);
			this.SerializeSpatialTransform(iod.SpatialTransform, imagesByList);
			this.SerializeGraphicLayer(iod.GraphicLayer, imagesByList);
			this.SerializeIccProfile(iod.IccProfile);
		}

		private void SerializeIccProfile(IccProfileModuleIod module) {
			// TODO : fix this dummy implementation
		}

		#endregion

		#region Deserialization Support

		protected override void PerformTypeSpecificDeserialization(IList<DicomColorPresentationImage> imagesByList, IDictionary<string, IList<DicomColorPresentationImage>> imagesBySeries) {
			ColorSoftcopyPresentationStateIod iod = new ColorSoftcopyPresentationStateIod(base.DataSet);

			foreach (DicomColorPresentationImage image in imagesByList) {
				RectangleF displayedArea;
				this.DeserializeSpatialTransform(iod.SpatialTransform, image);
				this.DeserializeDisplayedArea(iod.DisplayedArea, out displayedArea, image);
				this.DeserializeGraphicLayer(iod.GraphicLayer, image);
				this.DeserializeGraphicAnnotation(iod.GraphicAnnotation, displayedArea, image);

				image.Draw();
			}
		}

		#endregion

		#region IDicomAttributeProvider Copy Method

		private static DicomAttributeCollection ShallowCopyDataSource(IDicomAttributeProvider source) {
			if (source is DicomAttributeCollection)
				return (DicomAttributeCollection)source;

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

			foreach (uint tag in IccProfileModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SopCommonModuleIod.DefinedTags)
				collection[tag] = source[tag];

			return collection;
		}

		#endregion
	}
}
