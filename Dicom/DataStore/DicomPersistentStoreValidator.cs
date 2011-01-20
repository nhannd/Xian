#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Validation;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private sealed class DicomPersistentStoreValidator : IDicomPersistentStoreValidator
		{
			private readonly PersistentObjectValidator _validator;

			public DicomPersistentStoreValidator()
			{
				_validator = new PersistentObjectValidator(HibernateConfiguration);
			}

			#region IDicomPersistentStoreValidator Members

			public void Validate(DicomFile dicomFile)
			{
				const string metaInfoErrorMessageFormat = "Invalid DICOM File Meta Info ({0})";

				DicomAttributeCollection sopInstanceDataset = dicomFile.DataSet;
				DicomAttributeCollection metaInfo = dicomFile.MetaInfo;

				DicomAttribute attribute = metaInfo[DicomTags.MediaStorageSopClassUid];
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == attribute.ToString())
					throw new DataStoreException("Cannot process DICOM directory files.");

				DicomValidator.ValidateStudyInstanceUID(sopInstanceDataset[DicomTags.StudyInstanceUid]);
				DicomValidator.ValidateSeriesInstanceUID(sopInstanceDataset[DicomTags.SeriesInstanceUid]);
				DicomValidator.ValidateSOPInstanceUID(sopInstanceDataset[DicomTags.SopInstanceUid]);
				DicomValidator.ValidateTransferSyntaxUID(metaInfo[DicomTags.TransferSyntaxUid]);

				if (dicomFile.SopClass == null)
					throw new DataStoreException(string.Format(metaInfoErrorMessageFormat, "SOP Class UID is missing"));

				try
				{
					DicomValidator.ValidateSopClassUid(dicomFile.SopClass.Uid);
				}
				catch (DicomDataException ex)
				{
					throw new DataStoreException(string.Format(metaInfoErrorMessageFormat, "SOP Class UID is missing"), ex);
				}

				Study study = new Study();
				study.Initialize(dicomFile);

				_validator.ValidatePersistentObject(study);
			}

			#endregion
		}
	}
}
