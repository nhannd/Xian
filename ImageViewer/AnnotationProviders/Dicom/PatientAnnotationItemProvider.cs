#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public PatientAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.Patient", new AnnotationResourceResolver(typeof(PatientAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.EthnicGroup",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.EthnicGroup),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientComments",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PatientComments),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientId",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientId; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsBirthDate",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsBirthDate; },
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsBirthTime",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PatientsBirthTime),
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName>
					(
						"Dicom.Patient.PatientsName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsName; },
						DicomDataFormatHelper.PersonNameFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsSex",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsSex; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName>
					(
						"Dicom.Patient.ResponsiblePerson",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.ResponsiblePerson; },
						DicomDataFormatHelper.PersonNameFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.ResponsibleOrganization",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.ResponsibleOrganization; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
					(
						"Dicom.Patient.PatientSpecies",
						resolver,
						DicomTags.PatientSpeciesCodeSequence,
						DicomTags.PatientSpeciesDescription
					)
				);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
					(
						"Dicom.Patient.PatientBreed",
						resolver,
						DicomTags.PatientBreedCodeSequence,
						DicomTags.PatientBreedDescription
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
