using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.PresentationStateRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Iod.Sequences;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[Cloneable]
	internal abstract class DicomSoftcopyPresentationStateBase<T> : DicomSoftcopyPresentationState where T : IPresentationImage, IImageSopProvider, ISpatialTransformProvider, IImageGraphicProvider, IOverlayGraphicsProvider
	{
		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass) : base(psSopClass) {}

		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass, DicomFile dicomFile) : base(psSopClass, dicomFile) {}

		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass, DicomAttributeCollection dataSource) : base(psSopClass, dataSource) {}

		protected DicomSoftcopyPresentationStateBase(DicomSoftcopyPresentationStateBase<T> source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override void PerformSerialization(IEnumerable<IPresentationImage> images)
		{
			List<T> listImages = new List<T>();
			Dictionary<string, IList<T>> seriesImages = new Dictionary<string, IList<T>>();
			foreach (IPresentationImage image in images)
			{
				if (image is T)
				{
					T tImage = (T) image;
					string seriesUid = tImage.ImageSop.SeriesInstanceUID;
					if (!seriesImages.ContainsKey(seriesUid))
						seriesImages.Add(seriesUid, new List<T>());
					seriesImages[seriesUid].Add(tImage);
					listImages.Add(tImage);
				}
			}

			if (listImages.Count == 0)
				return;

			// Initialize the Patient IE using source image information
			InitializePatientModule(new PatientModuleIod(base.DataSet), listImages[0]);
			InitializeClinicalTrialSubjectModule(new ClinicalTrialSubjectModuleIod(base.DataSet), listImages[0]);

			// Initialize the Study IE using source image information
			InitializeGeneralStudyModule(new GeneralStudyModuleIod(base.DataSet), listImages[0]);
			InitializePatientStudyModule(new PatientStudyModuleIod(base.DataSet), listImages[0]);
			InitializeClinicalTrialStudyModule(new ClinicalTrialStudyModuleIod(base.DataSet), listImages[0]);

			this.PerformTypeSpecificSerialization(listImages, seriesImages);
		}

		protected override sealed void PerformDeserialization(IEnumerable<IPresentationImage> images)
		{
			foreach (PresentationStateRelationshipModuleIod psRelationship in this.RelationshipSets)
			{
				SeriesReferenceDictionary dictionary = new SeriesReferenceDictionary(psRelationship.ReferencedSeriesSequence);

				List<T> listImages = new List<T>();
				Dictionary<string, IList<T>> seriesImages = new Dictionary<string, IList<T>>();
				foreach (IPresentationImage image in images)
				{
					if (image is T)
					{
						T tImage = (T) image;

						string seriesUid = tImage.ImageSop.SeriesInstanceUID;
						if (dictionary.ReferencesFrame(seriesUid, tImage.ImageSop.SopInstanceUID, tImage.Frame.FrameNumber))
						{
							if (!seriesImages.ContainsKey(seriesUid))
								seriesImages.Add(seriesUid, new List<T>());
							seriesImages[seriesUid].Add(tImage);
							listImages.Add(tImage);
						}
					}
				}

				this.PerformTypeSpecificDeserialization(listImages, seriesImages);
			}
		}

		protected abstract void PerformTypeSpecificSerialization(IList<T> imagesByList, IDictionary<string, IList<T>> imagesBySeries);
		protected abstract void PerformTypeSpecificDeserialization(IList<T> imagesByList, IDictionary<string, IList<T>> imagesBySeries);

		/// <summary>
		/// Gets a <see cref="PresentationStateRelationshipModuleIod"/> for this data set.
		/// </summary>
		/// <remarks>
		/// As of the 2008 version of the DICOM standard, only the Blended Softcopy Presentation State IOD defines
		/// multiple presentation state relationship modules (as part of the presentation state blending module).
		/// Thus, only the implementation of the Blended Softcopy Presentation State should override this member
		/// to provide all the individual relationship modules. The default implementation assumes the module
		/// is available as a root member of the IOD.
		/// </remarks>
		protected virtual IEnumerable<PresentationStateRelationshipModuleIod> RelationshipSets
		{
			get { return new PresentationStateRelationshipModuleIod[] {new PresentationStateRelationshipModuleIod(this.DataSet)}; }
		}

		#region Serialization of Demographic/Study Data

		private static void InitializePatientModule(PatientModuleIod patientModule, T prototypeImage)
		{
			PatientModuleIod srcPatient = new PatientModuleIod(prototypeImage.ImageSop.DataSource);
			patientModule.BreedRegistrationSequence = srcPatient.BreedRegistrationSequence;
			patientModule.DeIdentificationMethod = srcPatient.DeIdentificationMethod;
			patientModule.DeIdentificationMethodCodeSequence = srcPatient.DeIdentificationMethodCodeSequence;
			patientModule.EthnicGroup = srcPatient.EthnicGroup;
			patientModule.IssuerOfPatientId = srcPatient.IssuerOfPatientId;
			patientModule.OtherPatientIds = srcPatient.OtherPatientIds;
			patientModule.OtherPatientIdsSequence = srcPatient.OtherPatientIdsSequence;
			patientModule.OtherPatientNames = srcPatient.OtherPatientNames;
			patientModule.PatientBreedCodeSequence = srcPatient.PatientBreedCodeSequence;
			patientModule.PatientBreedDescription = srcPatient.PatientBreedDescription;
			patientModule.PatientComments = srcPatient.PatientComments;
			patientModule.PatientId = srcPatient.PatientId;
			patientModule.PatientIdentityRemoved = srcPatient.PatientIdentityRemoved;
			patientModule.PatientsBirthDateTime = srcPatient.PatientsBirthDateTime;
			patientModule.PatientsName = srcPatient.PatientsName;
			patientModule.PatientSpeciesCodeSequence = srcPatient.PatientSpeciesCodeSequence;
			patientModule.PatientSpeciesDescription = srcPatient.PatientSpeciesDescription;
			patientModule.PatientsSex = srcPatient.PatientsSex;
			patientModule.ReferencedPatientSequence = srcPatient.ReferencedPatientSequence;
			patientModule.ResponsibleOrganization = srcPatient.ResponsibleOrganization;
			patientModule.ResponsiblePerson = srcPatient.ResponsiblePerson;
			patientModule.ResponsiblePersonRole = srcPatient.ResponsiblePersonRole;
		}

		private static void InitializeClinicalTrialSubjectModule(ClinicalTrialSubjectModuleIod clinicalTrialSubjectModule, T prototypeImage)
		{
			ClinicalTrialSubjectModuleIod srcTrialSubject = new ClinicalTrialSubjectModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				clinicalTrialSubjectModule.ClinicalTrialProtocolId = srcTrialSubject.ClinicalTrialProtocolId;
				clinicalTrialSubjectModule.ClinicalTrialProtocolName = srcTrialSubject.ClinicalTrialProtocolName;
				clinicalTrialSubjectModule.ClinicalTrialSiteId = srcTrialSubject.ClinicalTrialSiteId;
				clinicalTrialSubjectModule.ClinicalTrialSiteName = srcTrialSubject.ClinicalTrialSiteName;
				clinicalTrialSubjectModule.ClinicalTrialSponsorName = srcTrialSubject.ClinicalTrialSponsorName;
				clinicalTrialSubjectModule.ClinicalTrialSubjectId = srcTrialSubject.ClinicalTrialSubjectId;
				clinicalTrialSubjectModule.ClinicalTrialSubjectReadingId = srcTrialSubject.ClinicalTrialSubjectReadingId;
			}
		}

		private static void InitializeGeneralStudyModule(GeneralStudyModuleIod generalStudyModule, T prototypeImage)
		{
			GeneralStudyModuleIod srcGeneralStudy = new GeneralStudyModuleIod(prototypeImage.ImageSop.DataSource);
			generalStudyModule.AccessionNumber = srcGeneralStudy.AccessionNumber;
			generalStudyModule.NameOfPhysiciansReadingStudy = srcGeneralStudy.NameOfPhysiciansReadingStudy;
			generalStudyModule.PhysiciansOfRecord = srcGeneralStudy.PhysiciansOfRecord;
			generalStudyModule.PhysiciansOfRecordIdentificationSequence = srcGeneralStudy.PhysiciansOfRecordIdentificationSequence;
			generalStudyModule.PhysiciansReadingStudyIdentificationSequence = srcGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
			generalStudyModule.ProcedureCodeSequence = srcGeneralStudy.ProcedureCodeSequence;
			generalStudyModule.ReferencedStudySequence = srcGeneralStudy.ReferencedStudySequence;
			generalStudyModule.ReferringPhysicianIdentificationSequence = srcGeneralStudy.ReferringPhysicianIdentificationSequence;
			generalStudyModule.ReferringPhysiciansName = srcGeneralStudy.ReferringPhysiciansName;
			generalStudyModule.StudyDateTime = srcGeneralStudy.StudyDateTime;
			generalStudyModule.StudyDescription = srcGeneralStudy.StudyDescription;
			generalStudyModule.StudyId = srcGeneralStudy.StudyId;
			generalStudyModule.StudyInstanceUid = srcGeneralStudy.StudyInstanceUid;
		}

		private static void InitializePatientStudyModule(PatientStudyModuleIod patientStudyModule, T prototypeImage)
		{
			PatientStudyModuleIod srcPatientStudy = new PatientStudyModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcPatientStudy.HasValues()) // patient study module is user optional
			{
				patientStudyModule.AdditionalPatientHistory = srcPatientStudy.AdditionalPatientHistory;
				patientStudyModule.AdmissionId = srcPatientStudy.AdmissionId;
				patientStudyModule.AdmittingDiagnosesCodeSequence = srcPatientStudy.AdmittingDiagnosesCodeSequence;
				patientStudyModule.AdmittingDiagnosesDescription = srcPatientStudy.AdmittingDiagnosesDescription;
				patientStudyModule.IssuerOfAdmissionId = srcPatientStudy.IssuerOfAdmissionId;
				patientStudyModule.IssuerOfServiceEpisodeId = srcPatientStudy.IssuerOfServiceEpisodeId;
				patientStudyModule.Occupation = srcPatientStudy.Occupation;
				patientStudyModule.PatientsAge = srcPatientStudy.PatientsAge;
				patientStudyModule.PatientsSexNeutered = srcPatientStudy.PatientsSexNeutered;
				patientStudyModule.PatientsSize = srcPatientStudy.PatientsSize;
				patientStudyModule.PatientsWeight = srcPatientStudy.PatientsWeight;
				patientStudyModule.ServiceEpisodeDescription = srcPatientStudy.ServiceEpisodeDescription;
				patientStudyModule.ServiceEpisodeId = srcPatientStudy.ServiceEpisodeId;
			}
		}

		private static void InitializeClinicalTrialStudyModule(ClinicalTrialStudyModuleIod clinicalTrialStudyModule, T prototypeImage)
		{
			ClinicalTrialStudyModuleIod srcTrialStudy = new ClinicalTrialStudyModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				clinicalTrialStudyModule.ClinicalTrialTimePointDescription = srcTrialStudy.ClinicalTrialTimePointDescription;
				clinicalTrialStudyModule.ClinicalTrialTimePointId = srcTrialStudy.ClinicalTrialTimePointId;
			}
		}

		#endregion

		#region Serialization of Presentation States

		private DisplayAreaSerializationOption _displayAreaSerializationOption = DisplayAreaSerializationOption.SerializeAsDisplayedArea;

		public DisplayAreaSerializationOption DisplayAreaSerializationOption
		{
			get { return _displayAreaSerializationOption; }
			set { _displayAreaSerializationOption = value; }
		}

		protected void SerializePresentationStateRelationship(PresentationStateRelationshipModuleIod presentationStateRelationshipModule, IDictionary<string, IList<T>> imagesBySeries)
		{
			presentationStateRelationshipModule.InitializeAttributes();
			List<IReferencedSeriesSequence> seriesReferences = new List<IReferencedSeriesSequence>();
			foreach (string seriesUid in imagesBySeries.Keys)
			{
				IReferencedSeriesSequence seriesReference = presentationStateRelationshipModule.CreateReferencedSeriesSequence();
				seriesReference.SeriesInstanceUid = seriesUid;
				List<ImageSopInstanceReferenceMacro> imageReferences = new List<ImageSopInstanceReferenceMacro>();
				foreach (T image in imagesBySeries[seriesUid])
				{
					imageReferences.Add(CreateImageSopInstanceReference(image.Frame));
				}
				seriesReference.ReferencedImageSequence = imageReferences.ToArray();
				seriesReferences.Add(seriesReference);
			}
			presentationStateRelationshipModule.ReferencedSeriesSequence = seriesReferences.ToArray();
		}

		protected void SerializePresentationStateShutter(PresentationStateShutterModuleIod presentationStateShutterModule)
		{
			presentationStateShutterModule.InitializeAttributes();
		}

		protected void SerializeDisplayedArea(DisplayedAreaModuleIod displayedAreaModule, IList<T> images)
		{
			displayedAreaModule.InitializeAttributes();
			List<DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem> displayedAreas = new List<DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem>();
			foreach (T image in images)
			{
				DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem displayedArea = new DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem();
				displayedArea.InitializeAttributes();
				displayedArea.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};

				if (image is IImageGraphicProvider)
				{
					ImageGraphic imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
					Size imageSize = new Size(imageGraphic.Columns, imageGraphic.Rows);

					// compute the visible boundaries of the image as a positive rectangle in screen space
					RectangleF visibleBounds = imageGraphic.SpatialTransform.ConvertToDestination(new Rectangle(new Point(0, 0), imageSize));
					visibleBounds = RectangleUtilities.Intersect(visibleBounds, image.ClientRectangle);
					visibleBounds = RectangleUtilities.ConvertToPositiveRectangle(visibleBounds);

					// compute the visible area of the image as a rectangle oriented positively in screen space
					RectangleF visibleImageArea = imageGraphic.SpatialTransform.ConvertToSource(visibleBounds);
					visibleImageArea = RectangleUtilities.RoundInflate(visibleImageArea);

					// compute the pixel addresses of the visible area by intersecting area with actual pixel addresses available
					//Rectangle visiblePixels = Rectangle.Truncate(RectangleUtilities.Intersect(visibleImageArea, new RectangleF(_point1, imageSize)));
					Rectangle visiblePixels = RectangleUtilities.ConvertToPixelAddressRectangle(Rectangle.Truncate(visibleImageArea));
					displayedArea.DisplayedAreaTopLeftHandCorner = visiblePixels.Location;
					displayedArea.DisplayedAreaBottomRightHandCorner = visiblePixels.Location + visiblePixels.Size;
				}
				else
				{
					displayedArea.DisplayedAreaTopLeftHandCorner = image.ClientRectangle.Location + new Size(1, 1);
					displayedArea.DisplayedAreaBottomRightHandCorner = image.ClientRectangle.Location + image.ClientRectangle.Size;
				}

				ISpatialTransform spatialTransform = image.SpatialTransform;
				switch (_displayAreaSerializationOption)
				{
					case DisplayAreaSerializationOption.SerializeAsMagnification:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.Magnify;
						displayedArea.PresentationPixelMagnificationRatio = spatialTransform.Scale;
						break;
					case DisplayAreaSerializationOption.SerializeAsTrueSize:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.TrueSize;
						displayedArea.PresentationPixelSpacing = image.Frame.PixelSpacing;
						break;
					case DisplayAreaSerializationOption.SerializeAsDisplayedArea:
					default:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.ScaleToFit;
						break;
				}

				PixelAspectRatio pixelAspectRatio = image.Frame.PixelAspectRatio;
				if (pixelAspectRatio == null || pixelAspectRatio.IsNull)
					pixelAspectRatio = PixelAspectRatio.FromString(image.ImageSop[DicomTags.PixelAspectRatio].ToString());
				if (pixelAspectRatio == null || pixelAspectRatio.IsNull)
					pixelAspectRatio = new PixelAspectRatio(1, 1);
				displayedArea.PresentationPixelAspectRatio = pixelAspectRatio;

				displayedAreas.Add(displayedArea);
			}
			displayedAreaModule.DisplayedAreaSelectionSequence = displayedAreas.ToArray();
		}

		protected void SerializeSpatialTransform(SpatialTransformModuleIod spatialTransformModule, IList<T> images)
		{
			foreach (T image in images)
			{
				// spatial transform defines rotation in cartesian space - dicom module defines rotation as clockwise in image space
				// spatial transform defines both horizontal and vertical flip - dicom module defines horizontal flip only (vertical flip is 180 rotation plus horizontal flip)
				ISpatialTransform spatialTransform = image.SpatialTransform;
				int rotationBy90 = (spatialTransform.RotationXY%360)/90;
				int flipState = (spatialTransform.FlipX ? 2 : 0) + (spatialTransform.FlipY ? 1 : 0);
				spatialTransformModule.ImageRotation = _spatialTransformRotationTranslation[rotationBy90 + 4*flipState];
				spatialTransformModule.ImageHorizontalFlip = spatialTransform.FlipY ^ spatialTransform.FlipX ? ImageHorizontalFlip.Y : ImageHorizontalFlip.N;
				break;
			}
		}

		private static readonly int[] _spatialTransformRotationTranslation = new int[] {0, 270, 180, 90, 0, 90, 180, 270, 180, 270, 0, 90, 180, 90, 0, 270};

		private static readonly string _roiGraphicLayerId = "ROIGRAPHICS";

		protected void SerializeGraphicLayer(GraphicLayerModuleIod graphicLayerModule, IList<T> images)
		{
			Dictionary<string, string> layerIndex = new Dictionary<string, string>();
			List<GraphicLayerSequenceItem> layerSequences = new List<GraphicLayerSequenceItem>();

			int order = 1;
			foreach (T image in images)
			{
				DicomSoftcopyPresentationStateGraphic psGraphic = DicomSoftcopyPresentationStateGraphic.GetPresentationStateGraphic(image, false);
				if (psGraphic != null)
				{
					foreach (DicomSoftcopyPresentationStateGraphic.LayerGraphic layerGraphic in psGraphic)
					{
						if (!layerIndex.ContainsKey(layerGraphic.Id))
						{
							GraphicLayerSequenceItem layerSequence = new GraphicLayerSequenceItem();
							layerSequence.GraphicLayer = layerGraphic.Id.ToUpperInvariant();
							layerSequence.GraphicLayerDescription = layerGraphic.Description;
							layerSequence.GraphicLayerOrder = order++;
							layerSequence.GraphicLayerRecommendedDisplayCielabValue = layerGraphic.DisplayCIELabColor;
							layerSequence.GraphicLayerRecommendedDisplayGrayscaleValue = layerGraphic.DisplayGrayscaleColor;
							layerSequences.Add(layerSequence);
							layerIndex.Add(layerGraphic.Id, null);
						}
					}
				}

				foreach (IGraphic graphic in image.OverlayGraphics)
				{
					if (graphic is AnnotationGraphic)
					{
						if (!layerIndex.ContainsKey(_roiGraphicLayerId))
						{
							layerIndex.Add(_roiGraphicLayerId, null);
							GraphicLayerSequenceItem layerSequence = new GraphicLayerSequenceItem();
							layerSequence.GraphicLayer = _roiGraphicLayerId;
							layerSequence.GraphicLayerOrder = order++;
							layerSequences.Add(layerSequence);
							break;
						}
					}
				}
			}

			if (layerSequences.Count > 0)
				graphicLayerModule.GraphicLayerSequence = layerSequences.ToArray();
		}

		protected void SerializeGraphicAnnotation(GraphicAnnotationModuleIod graphicAnnotationModule, IList<T> images)
		{
			List<GraphicAnnotationSequenceItem> annotations = new List<GraphicAnnotationSequenceItem>();

			foreach (T image in images)
			{
				DicomSoftcopyPresentationStateGraphic psGraphic = DicomSoftcopyPresentationStateGraphic.GetPresentationStateGraphic(image, false);
				if (psGraphic != null)
				{
					foreach (DicomSoftcopyPresentationStateGraphic.LayerGraphic layerGraphic in psGraphic)
					{
						foreach (IGraphic graphic in layerGraphic.Graphics)
						{
							GraphicAnnotationSequenceItem annotation = new GraphicAnnotationSequenceItem();
							if (GraphicAnnotationSerializer.SerializeGraphic(graphic, annotation))
							{
								annotation.GraphicLayer = layerGraphic.Id.ToUpperInvariant();
								annotation.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};
								annotations.Add(annotation);
							}
						}
					}
				}

				foreach (IGraphic graphic in image.OverlayGraphics)
				{
					GraphicAnnotationSequenceItem annotation = new GraphicAnnotationSequenceItem();
					if (GraphicAnnotationSerializer.SerializeGraphic(graphic, annotation))
					{
						annotation.GraphicLayer = _roiGraphicLayerId;
						annotation.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};
						annotations.Add(annotation);
					}
				}
			}

			if (annotations.Count > 0)
				graphicAnnotationModule.GraphicAnnotationSequence = annotations.ToArray();
		}

		#endregion

		#region Unimplemented Serializers

		protected void SerializeDisplayShutter(DisplayShutterModuleIod displayShutterModule)
		{
			//TODO: since we only do presentation states for key objects right now, implementing this
			//would cause inconsistent behaviour.  When we support presentation states fully, this should be implemented,
			//along with deserialization.
		}

		protected void SerializeBitmapDisplayShutter(BitmapDisplayShutterModuleIod bitmapDisplayShutterModule)
		{
			//TODO: since we only do presentation states for key objects right now, implementing this
			//would cause inconsistent behaviour.  When we support presentation states fully, this should be implemented,
			//along with deserialization.
		}

		protected void SerializeOverlayPlane(OverlayPlaneModuleIod overlayPlaneModule)
		{
			// TODO : fix this dummy implementation
		}

		protected void SerializeOverlayActivation(OverlayActivationModuleIod overlayActivationModule)
		{
			// TODO : fix this dummy implementation
		}

		#endregion

		#region Deserialization of Presentation States

		protected void DeserializeDisplayedArea(DisplayedAreaModuleIod dispAreaMod, out RectangleF displayedArea, T image)
		{
			ISpatialTransform spatialTransform = image.SpatialTransform;
			foreach (DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem item in dispAreaMod.DisplayedAreaSelectionSequence)
			{
				if (item.ReferencedImageSequence[0].ReferencedSopInstanceUid == image.ImageSop.SopInstanceUID)
				{
					RectangleF displayRect = new RectangleF(item.DisplayedAreaTopLeftHandCorner, new Size(item.DisplayedAreaBottomRightHandCorner - new Size(item.DisplayedAreaTopLeftHandCorner)));
					displayRect = RectangleUtilities.ConvertToPositiveRectangle(displayRect);
					displayRect.Location = displayRect.Location - new SizeF(1, 1);

					switch (item.PresentationSizeMode)
					{
						case DisplayedAreaModuleIod.PresentationSizeMode.Magnify:
							// displays selected area at specified magnification factor
							spatialTransform.Scale = (float) item.PresentationPixelMagnificationRatio.GetValueOrDefault(1);
							break;
						case DisplayedAreaModuleIod.PresentationSizeMode.TrueSize:
							// currently no support for determining true size, so default to scale area to fit
						case DisplayedAreaModuleIod.PresentationSizeMode.ScaleToFit:
						case DisplayedAreaModuleIod.PresentationSizeMode.None:
						default:
							if (spatialTransform is IImageSpatialTransform && displayRect.Location == new PointF(0, 0) && displayRect.Size == new SizeF(image.ImageGraphic.Columns, image.ImageGraphic.Rows))
							{
								// if the display rect is the whole image, then take advantage of the built-in scale image to fit functionality
								IImageSpatialTransform iist = (IImageSpatialTransform) spatialTransform;
								iist.ScaleToFit = true;
							}
							else
							{
								// otherwise manually compute max magnification to show all off selected area
								SizeF clientArea = image.ClientRectangle.Size;
								if (spatialTransform.RotationXY%180 > 0)
									clientArea = new SizeF(clientArea.Height, clientArea.Width);
								spatialTransform.Scale = Math.Min(clientArea.Width/displayRect.Width, clientArea.Height/displayRect.Height);
							}

							break;
					}

					// center the area on the tile
					spatialTransform.TranslationX = (item.DisplayedAreaTopLeftHandCorner.X + item.DisplayedAreaBottomRightHandCorner.X - image.ImageGraphic.Columns)/-2f;
					spatialTransform.TranslationY = (item.DisplayedAreaTopLeftHandCorner.Y + item.DisplayedAreaBottomRightHandCorner.Y - image.ImageGraphic.Rows)/-2f;

					displayedArea = displayRect;
					return;
				}
			}
			displayedArea = new RectangleF(0, 0, image.ImageGraphic.Columns, image.ImageGraphic.Rows);
		}

		protected void DeserializeSpatialTransform(SpatialTransformModuleIod module, T image)
		{
			ISpatialTransform spatialTransform = image.SpatialTransform;
			if (spatialTransform is IImageSpatialTransform)
			{
				IImageSpatialTransform iist = (IImageSpatialTransform) spatialTransform;
				iist.ScaleToFit = false;
			}

			if (module.ImageHorizontalFlip == ImageHorizontalFlip.Y)
			{
				spatialTransform.FlipY = true;
				spatialTransform.RotationXY = module.ImageRotation;
			}
			else
			{
				spatialTransform.FlipY = false;
				spatialTransform.RotationXY = (360 - module.ImageRotation)%360;
			}
		}

		protected void DeserializeGraphicLayer(GraphicLayerModuleIod module, T image)
		{
			GraphicLayerSequenceItem[] layerSequences = module.GraphicLayerSequence;
			if (layerSequences == null)
				return;

			SortedDictionary<int, GraphicLayerSequenceItem> orderedSequences = new SortedDictionary<int, GraphicLayerSequenceItem>();
			foreach (GraphicLayerSequenceItem sequenceItem in layerSequences)
			{
				orderedSequences.Add(sequenceItem.GraphicLayerOrder, sequenceItem);
			}

			DicomSoftcopyPresentationStateGraphic graphic = DicomSoftcopyPresentationStateGraphic.GetPresentationStateGraphic(image, true);
			foreach (GraphicLayerSequenceItem sequenceItem in orderedSequences.Values)
			{
				DicomSoftcopyPresentationStateGraphic.LayerGraphic layer = graphic.AddLayer(sequenceItem.GraphicLayer);
				layer.Description = sequenceItem.GraphicLayerDescription;
				layer.DisplayCIELabColor = sequenceItem.GraphicLayerRecommendedDisplayCielabValue;
				layer.DisplayGrayscaleColor = sequenceItem.GraphicLayerRecommendedDisplayGrayscaleValue;
			}
		}

		protected void DeserializeGraphicAnnotation(GraphicAnnotationModuleIod module, RectangleF displayedArea, T image)
		{
			GraphicAnnotationSequenceItem[] annotationSequences = module.GraphicAnnotationSequence;
			if (annotationSequences == null)
				return;

			DicomSoftcopyPresentationStateGraphic graphic = DicomSoftcopyPresentationStateGraphic.GetPresentationStateGraphic(image, true);
			foreach (GraphicAnnotationSequenceItem sequenceItem in annotationSequences)
			{
				ImageSopInstanceReferenceDictionary dictionary = new ImageSopInstanceReferenceDictionary(sequenceItem.ReferencedImageSequence, true);
				if (dictionary.ReferencesFrame(image.ImageSop.SopInstanceUID, image.Frame.FrameNumber))
				{
					DicomGraphicAnnotation annotation = new DicomGraphicAnnotation(sequenceItem, displayedArea);
					graphic[sequenceItem.GraphicLayer].Graphics.Add(annotation);
				}
			}
		}

		#endregion
	}
}