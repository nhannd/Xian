using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Iod.Iods
{
	public class GrayscaleSoftcopyPresentationStateIod
	{
		private readonly DicomAttributeCollection _dataset;

		public GrayscaleSoftcopyPresentationStateIod() : this(new DicomAttributeCollection()) {}

		public GrayscaleSoftcopyPresentationStateIod(DicomAttributeCollection dataset)
		{
			_dataset = dataset;

			this.Patient = new PatientModuleIod(dataset);
			this.ClinicalTrialSubject = new ClinicalTrialSubjectModuleIod(dataset);

			this.GeneralStudy = new GeneralStudyModuleIod(dataset);
			this.PatientStudy = new PatientStudyModuleIod(dataset);
			this.ClinicalTrialStudy = new ClinicalTrialStudyModuleIod(dataset);

			this.GeneralSeries = new GeneralSeriesModuleIod(dataset);
			this.ClinicalTrialSeries = new ClinicalTrialSeriesModuleIod(dataset);
			this.PresentationSeries = new PresentationSeriesModuleIod(dataset);

			this.GeneralEquipment = new GeneralEquipmentModuleIod(dataset);

			this.PresentationStateIdentification = new PresentationStateIdentificationModuleIod(dataset);
			this.PresentationStateRelationship = new PresentationStateRelationshipModuleIod(dataset);
			this.PresentationStateShutter = new PresentationStateShutterModuleIod(dataset);
			this.PresentationStateMask = new PresentationStateMaskModuleIod(dataset);
			this.Mask = new MaskModuleIod(dataset);
			this.DisplayShutter = new DisplayShutterModuleIod(dataset);
			this.BitmapDisplayShutter = new BitmapDisplayShutterModuleIod(dataset);
			this.OverlayPlane = new OverlayPlaneModuleIod(dataset);
			this.OverlayActivation = new OverlayActivationModuleIod(dataset);
			this.DisplayedArea = new DisplayedAreaModuleIod(dataset);
			this.GraphicAnnotation = new GraphicAnnotationModuleIod(dataset);
			this.SpatialTransform = new SpatialTransformModuleIod(dataset);
			this.GraphicLayer = new GraphicLayerModuleIod(dataset);
			this.ModalityLut = new ModalityLutModuleIod(dataset);
			this.SoftcopyVoiLut = new SoftcopyVoiLutModuleIod(dataset);
			this.SoftcopyPresentationLut = new SoftcopyPresentationLutModuleIod(dataset);
			this.SopCommon = new SopCommonModuleIod(dataset);
		}

		public DicomAttributeCollection DataSet
		{
			get { return _dataset; }
		}

		#region Patient IE

		/// <summary>
		/// Gets the Patient module (required usage).
		/// </summary>
		public readonly PatientModuleIod Patient;

		/// <summary>
		/// Gets the Clinical Trial Subject module (optional usage).
		/// </summary>
		public readonly ClinicalTrialSubjectModuleIod ClinicalTrialSubject;

		#endregion

		#region Study IE

		/// <summary>
		/// Gets the General Study module (required usage).
		/// </summary>
		public readonly GeneralStudyModuleIod GeneralStudy;

		/// <summary>
		/// Gets the Patient Study module (optional usage).
		/// </summary>
		public readonly PatientStudyModuleIod PatientStudy;

		/// <summary>
		/// Gets the Clinical Trial Study module (optional usage).
		/// </summary>
		public readonly ClinicalTrialStudyModuleIod ClinicalTrialStudy;

		#endregion

		#region Series IE

		/// <summary>
		/// Gets the General Series module (required usage).
		/// </summary>
		public readonly GeneralSeriesModuleIod GeneralSeries;

		/// <summary>
		/// Gets the Clinical Trial Series module (optional usage).
		/// </summary>
		public readonly ClinicalTrialSeriesModuleIod ClinicalTrialSeries;

		/// <summary>
		/// Gets the Presentation Series module (required usage).
		/// </summary>
		public readonly PresentationSeriesModuleIod PresentationSeries;

		#endregion

		#region Equipment IE

		/// <summary>
		/// Gets the General Equipment module (required usage).
		/// </summary>
		public readonly GeneralEquipmentModuleIod GeneralEquipment;

		#endregion

		#region Presentation State IE

		/// <summary>
		/// Gets the Presentation State Identification module (required usage).
		/// </summary>
		public readonly PresentationStateIdentificationModuleIod PresentationStateIdentification;

		/// <summary>
		/// Gets the Presentation State Relationship module (required usage).
		/// </summary>
		public readonly PresentationStateRelationshipModuleIod PresentationStateRelationship;

		/// <summary>
		/// Gets the Presentation State Shutter module (required usage).
		/// </summary>
		public readonly PresentationStateShutterModuleIod PresentationStateShutter;

		/// <summary>
		/// Gets the Presentation State Mask module (required usage).
		/// </summary>
		public readonly PresentationStateMaskModuleIod PresentationStateMask;

		/// <summary>
		/// Gets the Mask module (required if the referenced images are multiframe and are to be subtracted).
		/// </summary>
		public readonly MaskModuleIod Mask;

		/// <summary>
		/// Gets the Display Shutter module (conditionally required usage).
		/// </summary>
		public readonly DisplayShutterModuleIod DisplayShutter;

		/// <summary>
		/// Gets the Bitmap Display Shutter module (conditionally required usage).
		/// </summary>
		public readonly BitmapDisplayShutterModuleIod BitmapDisplayShutter;

		/// <summary>
		/// Gets the Overlay Plane module (conditionally required usage).
		/// </summary>
		public readonly OverlayPlaneModuleIod OverlayPlane;

		/// <summary>
		/// Gets the Overlay Activation module (conditionally required usage).
		/// </summary>
		public readonly OverlayActivationModuleIod OverlayActivation;

		/// <summary>
		/// Gets the Displayed Area module (required usage).
		/// </summary>
		public readonly DisplayedAreaModuleIod DisplayedArea;

		/// <summary>
		/// Gets the Graphic Annotation module (conditionally required usage).
		/// </summary>
		public readonly GraphicAnnotationModuleIod GraphicAnnotation;

		/// <summary>
		/// Gets the Spatial Transform module (conditionally required usage).
		/// </summary>
		public readonly SpatialTransformModuleIod SpatialTransform;

		/// <summary>
		/// Gets the Graphic Layer module (conditionally required usage).
		/// </summary>
		public readonly GraphicLayerModuleIod GraphicLayer;

		/// <summary>
		/// Gets the Modality LUT module (conditionally required usage).
		/// </summary>
		public readonly ModalityLutModuleIod ModalityLut;

		/// <summary>
		/// Gets the Softcopy VOI LUT module (conditionally required usage).
		/// </summary>
		public readonly SoftcopyVoiLutModuleIod SoftcopyVoiLut;

		/// <summary>
		/// Gets the Softcopy Presentation LUT module (required usage).
		/// </summary>
		public readonly SoftcopyPresentationLutModuleIod SoftcopyPresentationLut;

		/// <summary>
		/// Gets the SOP Common module (required usage).
		/// </summary>
		public readonly SopCommonModuleIod SopCommon;

		#endregion
	}
}