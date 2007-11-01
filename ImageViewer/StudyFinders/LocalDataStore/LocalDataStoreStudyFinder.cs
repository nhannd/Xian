#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;

namespace ClearCanvas.ImageViewer.StudyFinders.LocalDataStore
{
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyFinderExtensionPoint))]
    public class LocalDataStoreStudyFinder : IStudyFinder
    {
        public LocalDataStoreStudyFinder()
        {

        }

        public string Name
        {
            get
            {
                return "DICOM_LOCAL";
            }
        }

        public StudyItemList Query(QueryParameters queryParams, object targetServer)
        {
			Platform.CheckForNullReference(queryParams, "queryParams");

            QueryKey queryKey = new QueryKey();
            queryKey.Add(DicomTags.PatientId, queryParams["PatientId"]);
            queryKey.Add(DicomTags.AccessionNumber, queryParams["AccessionNumber"]);
            queryKey.Add(DicomTags.PatientsName, queryParams["PatientsName"]);
            queryKey.Add(DicomTags.StudyDate, queryParams["StudyDate"]);
            queryKey.Add(DicomTags.StudyDescription, queryParams["StudyDescription"]);
            queryKey.Add(DicomTags.PatientsBirthDate, "");
            queryKey.Add(DicomTags.ModalitiesInStudy, queryParams["ModalitiesInStudy"]);
            queryKey.Add(DicomTags.SpecificCharacterSet, "");
			queryKey.Add(DicomTags.StudyInstanceUid, queryParams["StudyInstanceUid"]);

            ReadOnlyQueryResultCollection results = Query(queryKey);
            if (null == results)
                return null;

            StudyItemList studyItemList = new StudyItemList();
            foreach (QueryResult result in results)
            {
                StudyItem item = new StudyItem();
                item.SpecificCharacterSet = result.SpecificCharacterSet;
                item.PatientId = result.PatientId.ToString();
                item.PatientsName = result.PatientsName;
                item.PatientsBirthDate = result[DicomTags.PatientsBirthDate];
                item.StudyDate = result.StudyDate;
                item.StudyDescription = result.StudyDescription;
                item.ModalitiesInStudy = result.ModalitiesInStudy;
                item.AccessionNumber = result.AccessionNumber;
                item.StudyInstanceUID = result.StudyInstanceUid.ToString();
                item.StudyLoaderName = this.Name;

                studyItemList.Add(item);
            }

            return studyItemList;
        }

        protected ReadOnlyQueryResultCollection Query(QueryKey queryKey)
        {
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				return reader.StudyQuery(queryKey);
			}
        }       
    }
}
