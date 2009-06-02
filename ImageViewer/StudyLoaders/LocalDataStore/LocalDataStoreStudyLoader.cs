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

    	public override int OnStart(StudyLoaderArgs studyLoaderArgs)
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


