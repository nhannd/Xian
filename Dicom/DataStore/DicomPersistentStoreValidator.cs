using System;

namespace ClearCanvas.Dicom.DataStore
{
	public sealed partial class DataAccessLayer
	{
		private sealed class DicomPersistentStoreValidator : IDicomPersistentStoreValidator
		{
			private readonly PersistenObjectValidator _validator;

			public DicomPersistentStoreValidator()
			{
				_validator = new PersistenObjectValidator(HibernateConfiguration);
			}

			#region IDicomPersistentStoreValidator Members

			public void Validate(DicomAttributeCollection metaInfo, DicomAttributeCollection sopInstanceDataset)
			{
				DicomAttribute attribute = metaInfo[DicomTags.MediaStorageSopClassUid];
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == attribute.ToString())
					throw new DataStoreException(SR.ExceptionCannotProcessDicomDirFiles);

				DicomValidator.ValidateStudyInstanceUID(sopInstanceDataset[DicomTags.StudyInstanceUid]);
				DicomValidator.ValidateSeriesInstanceUID(sopInstanceDataset[DicomTags.SeriesInstanceUid]);
				DicomValidator.ValidateSOPInstanceUID(sopInstanceDataset[DicomTags.SopInstanceUid]);
				DicomValidator.ValidateTransferSyntaxUID(metaInfo[DicomTags.TransferSyntaxUid]);

				Study study = new Study();
				study.Update(metaInfo, sopInstanceDataset);

				Series series = new Series();
				series.Update(metaInfo, sopInstanceDataset);

				ImageSopInstance sop = new ImageSopInstance();
				sop.Update(metaInfo, sopInstanceDataset);

				_validator.ValidatePersistentObject(study);
				_validator.ValidatePersistentObject(series);
				_validator.ValidatePersistentObject(sop);
			}

			#endregion
		}
	}
}
