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
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.DataStore;
using System;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
	public class LocalDataStoreStudyLoader : StudyLoader
    {
		private IEnumerator<ISopInstance> _sops;

        public LocalDataStoreStudyLoader() : base("DICOM_LOCAL")
        {

        }

    	protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
		{
    		_sops = null;

    		EventResult result = EventResult.Success;
			AuditedInstances loadedInstances = new AuditedInstances();
			try
			{
				using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
				{
					IStudy study = reader.GetStudy(studyLoaderArgs.StudyInstanceUid);
					if (study == null)
					{
						result = EventResult.MajorFailure;
						loadedInstances.AddInstance(studyLoaderArgs.StudyInstanceUid);
						throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid);
					}
					loadedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

					_sops = study.GetSopInstances().GetEnumerator();
					return study.NumberOfStudyRelatedInstances;
				}
			}
			finally
			{
				AuditHelper.LogOpenStudies(new string[] { AuditHelper.LocalAETitle }, loadedInstances, EventSource.CurrentUser, result);
			}
		}

		protected override SopDataSource LoadNextSopDataSource()
        {
			if (_sops == null)
				return null;

			if (!_sops.MoveNext())
			{
				_sops = null;
				return null;
			}

			return new LocalDataStoreSopDataSource(_sops.Current);
        }
    }
}


