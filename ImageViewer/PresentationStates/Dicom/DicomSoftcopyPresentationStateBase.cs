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

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable]
	internal abstract class DicomSoftcopyPresentationStateBase<T> : DicomSoftcopyPresentationState where T : IDicomPresentationImage
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

		protected void SerializeDisplayedArea(DisplayedAreaModuleIod displayedAreaModule, IEnumerable<T> images)
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

		protected void SerializeSpatialTransform(SpatialTransformModuleIod spatialTransformModule, IEnumerable<T> images)
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

		protected void SerializeGraphicLayer(GraphicLayerModuleIod graphicLayerModule, IEnumerable<T> images)
		{
			Dictionary<string, string> layerIndex = new Dictionary<string, string>();
			List<GraphicLayerSequenceItem> layerSequences = new List<GraphicLayerSequenceItem>();

			int order = 1;
			foreach (T image in images)
			{
				DicomGraphicsPlane psGraphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (psGraphic != null)
				{
					foreach (LayerGraphic layerGraphic in (IEnumerable<LayerGraphic>)psGraphic.Layers)
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

		protected void SerializeGraphicAnnotation(GraphicAnnotationModuleIod graphicAnnotationModule, IEnumerable<T> images)
		{
			List<GraphicAnnotationSequenceItem> annotations = new List<GraphicAnnotationSequenceItem>();

			foreach (T image in images)
			{
				DicomGraphicsPlane psGraphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (psGraphic != null)
				{
					foreach (LayerGraphic layerGraphic in (IEnumerable<LayerGraphic>)psGraphic)
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

		protected void SerializeDisplayShutter(DisplayShutterModuleIod displayShutterModule, IEnumerable<T> images)
		{
			//TODO: since we only do presentation states for key objects right now, implementing this
			//would cause inconsistent behaviour.  When we support presentation states fully, this should be implemented,
			//along with deserialization.
		}

		protected void SerializeBitmapDisplayShutter(BitmapDisplayShutterModuleIod bitmapDisplayShutterModule, IEnumerable<T> images)
		{
			// TODO: Serialize user-created bitmap display shutters, when we support user-created overlay planes.
		}

		protected void SerializeOverlayPlane(OverlayPlaneModuleIod overlayPlaneModule, IEnumerable<T> images)
		{
			// TODO: Serialize user-created overlay planes, when we support user-created overlay planes.
		}

		protected void SerializeOverlayActivation(OverlayActivationModuleIod overlayActivationModule, IEnumerable<T> images)
		{
			OverlayPlaneSource?[] sources = new OverlayPlaneSource?[16];
			foreach (T image in images)
			{
				DicomGraphicsPlane dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if(dicomGraphics != null)
				{
					
				}
			}
		}

		#endregion

		#region Deserialization of Presentation States

		private bool _overlayPlanesDeserialized = false;

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
				spatialTransform.RotationXY = (360 - module.ImageRotation) % 360;// module.ImageRotation;
			}
			else
			{
				spatialTransform.FlipY = false;
				spatialTransform.RotationXY = module.ImageRotation;// (360 - module.ImageRotation) % 360;
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

			DicomGraphicsPlane graphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (GraphicLayerSequenceItem sequenceItem in orderedSequences.Values)
			{
				LayerGraphic layer = graphic.Layers.Add(sequenceItem.GraphicLayer);
				layer.Description = sequenceItem.GraphicLayerDescription;
				layer.DisplayCIELabColor = sequenceItem.GraphicLayerRecommendedDisplayCielabValue;
				layer.DisplayGrayscaleColor = sequenceItem.GraphicLayerRecommendedDisplayGrayscaleValue;
			}
		}

		protected void DeserializeGraphicAnnotation(GraphicAnnotationModuleIod module, RectangleF displayedArea, T image)
		{
			DicomGraphicsPlane graphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (DicomGraphicAnnotation annotation in DicomGraphicsFactory.CreateGraphicAnnotations(image.Frame, module, displayedArea))
			{
				graphic.Layers[annotation.LayerId].Graphics.Add(annotation);
			}
		}

		protected void DeserializeDisplayShutter(DisplayShutterModuleIod displayShutterModule, T image)
		{
			ShutterShape shape = displayShutterModule.ShutterShape;
			if (shape != ShutterShape.Bitmap && shape != ShutterShape.None)
			{
				IShutterGraphic shutter = DicomGraphicsFactory.CreateGeometricShuttersGraphic(displayShutterModule, image.Frame.Rows, image.Frame.Columns);
				// Some day, we will properly deserialize CIELab colours - until then, leave PresentationColor default black

				DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
				dicomGraphicsPlane.Shutters.Add(shutter);
				dicomGraphicsPlane.Shutters.Activate(shutter);
			}
		}

		/// <summary>
		/// Deserializes the specified bitmap display shutter module.
		/// </summary>
		/// <remarks>
		/// This method must be called after <see cref="DeserializeOverlayPlane">the overlay planes have been deserialized</see>.
		/// </remarks>
		/// <param name="bitmapDisplayShutterModule"></param>
		/// <param name="image"></param>
		protected void DeserializeBitmapDisplayShutter(BitmapDisplayShutterModuleIod bitmapDisplayShutterModule, T image)
		{
			if (!_overlayPlanesDeserialized)
				throw new InvalidOperationException("Overlay planes must be deserialized first.");

			if(bitmapDisplayShutterModule.ShutterShape == ShutterShape.Bitmap)
			{
				DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
				int overlayIndex = bitmapDisplayShutterModule.Index;
				if (overlayIndex >= 0 && overlayIndex < 16)
				{
					IShutterGraphic shutter = null;
					if (dicomGraphicsPlane.PresentationOverlays.Contains(overlayIndex))
					{
						shutter = dicomGraphicsPlane.PresentationOverlays[overlayIndex];
						dicomGraphicsPlane.PresentationOverlays.ActivateAsShutter(overlayIndex);
						dicomGraphicsPlane.ImageOverlays.Deactivate(overlayIndex);
					}
					else if (dicomGraphicsPlane.ImageOverlays.Contains(overlayIndex))
					{
						shutter = dicomGraphicsPlane.ImageOverlays[overlayIndex];
						dicomGraphicsPlane.ImageOverlays.ActivateAsShutter(overlayIndex);
					}

					// Some day, we will properly deserialize CIELab colours - until then, leave PresentationColor default black
				}
			}
		}

		protected void DeserializeOverlayPlane(OverlayPlaneModuleIod overlayPlaneModule, T image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (OverlayPlaneGraphic overlay in DicomGraphicsFactory.CreateOverlayPlaneGraphics(image.Frame, overlayPlaneModule))
			{
				// the results are a mix of overlays from the image itself and the presentation state
				if (overlay.Source == OverlayPlaneSource.Image)
					dicomGraphicsPlane.ImageOverlays.Add(overlay);
				else
					dicomGraphicsPlane.PresentationOverlays.Add(overlay);

				// the above lines will automatically add the overlays to the inactive layer
			}
			_overlayPlanesDeserialized = true;
		}

		/// <summary>
		/// Deserializes the specified overlay activation module.
		/// </summary>
		/// <remarks>
		/// This method must be called after <see cref="DeserializeOverlayPlane">the overlay planes have been deserialized</see>.
		/// </remarks>
		/// <param name="overlayActivationModule"></param>
		/// <param name="image"></param>
		protected void DeserializeOverlayActivation(OverlayActivationModuleIod overlayActivationModule, T image)
		{
			if (!_overlayPlanesDeserialized)
				throw new InvalidOperationException("Overlay planes must be deserialized first.");

			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			for (int n = 0; n < 16; n++)
			{
				if (overlayActivationModule.HasOverlayActivationLayer(n))
				{
					string targetLayer = overlayActivationModule[n].OverlayActivationLayer ?? string.Empty;
					if (dicomGraphicsPlane.PresentationOverlays.Contains(n))
					{
						if (string.IsNullOrEmpty(targetLayer))
							dicomGraphicsPlane.PresentationOverlays.Deactivate(n);
						else
							dicomGraphicsPlane.PresentationOverlays.ActivateAsLayer(n, targetLayer);
						dicomGraphicsPlane.ImageOverlays.Deactivate(n);
					}
					else if (dicomGraphicsPlane.ImageOverlays.Contains(n))
					{
						if (string.IsNullOrEmpty(targetLayer))
							dicomGraphicsPlane.ImageOverlays.Deactivate(n);
						else
							dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(n, targetLayer);
					}
				}
				else
				{
					// if the module is missing entirely, then the presentation state is poorly encoded.
					// for patient safety reasons, we override the DICOM stipulation that only one of
					// these two should be shown and show both instead.
					dicomGraphicsPlane.PresentationOverlays.ActivateAsLayer(n, "OVERLAY");
					dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(n, "OVERLAY");
				}
			}
		}

		#endregion
	}
}