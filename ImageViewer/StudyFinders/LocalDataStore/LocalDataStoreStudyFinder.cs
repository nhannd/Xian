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
            try
            {
				using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
				{
					return reader.StudyQuery(queryKey);
				}
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                // TODO
                Platform.ShowMessageBox(String.Format(SR.MessageUnableToConnectToDataStore, e.ToString()));
                return null;
            }
        }       
    }
}
