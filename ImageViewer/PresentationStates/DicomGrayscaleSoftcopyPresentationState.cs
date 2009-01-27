using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	internal sealed class DicomGrayscaleSoftcopyPresentationState : DicomSoftcopyPresentationStateBase<DicomGrayscalePresentationImage>
	{
		public static readonly SopClass SopClass = SopClass.GrayscaleSoftcopyPresentationStateStorageSopClass;

		public DicomGrayscaleSoftcopyPresentationState() : base(SopClass) {}

		public DicomGrayscaleSoftcopyPresentationState(DicomFile dicomFile) : base(SopClass, dicomFile) {}

		#region Serialization Support

		protected override void Serialize(IList<DicomGrayscalePresentationImage> imagesByList, IDictionary<string, IList<DicomGrayscalePresentationImage>> imagesBySeries)
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
			this.SerializeSoftcopyVoiLut(iod.SoftcopyVoiLut);
			this.SerializeSoftcopyPresentationLut(iod.SoftcopyPresentationLut);
		}

		private void SerializeSoftcopyPresentationLut(SoftcopyPresentationLutModuleIod module)
		{
			// TODO : fix this dummy implementation
			module.InitializeAttributes();
			module.PresentationLutShape = PresentationLutShape.Identity;
		}

		#endregion

		#region Deserialization Support

		protected override void Deserialize(IList<DicomGrayscalePresentationImage> imagesByList, IDictionary<string, IList<DicomGrayscalePresentationImage>> imagesBySeries)
		{
			GrayscaleSoftcopyPresentationStateIod iod = new GrayscaleSoftcopyPresentationStateIod(base.DataSet);

			foreach (DicomGrayscalePresentationImage image in imagesByList)
			{
				RectangleF displayedArea;
				this.DeserializeSpatialTransform(iod.SpatialTransform, image);
				this.DeserializeDisplayedArea(iod.DisplayedArea, out displayedArea, image);
				this.DeserializeGraphicLayer(iod.GraphicLayer, image);
				this.DeserializeGraphicAnnotation(iod.GraphicAnnotation, displayedArea, image);

				image.Draw();
			}
		}

		#endregion
	}
}