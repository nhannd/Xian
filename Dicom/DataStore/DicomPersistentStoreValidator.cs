#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
