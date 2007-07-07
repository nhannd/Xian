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
            queryKey.Add(DicomTag.PatientId, queryParams["PatientId"]);
            queryKey.Add(DicomTag.AccessionNumber, queryParams["AccessionNumber"]);
            queryKey.Add(DicomTag.PatientsName, queryParams["PatientsName"]);
            queryKey.Add(DicomTag.StudyDate, queryParams["StudyDate"]);
            queryKey.Add(DicomTag.StudyDescription, queryParams["StudyDescription"]);
            queryKey.Add(DicomTag.PatientsBirthDate, "");
            queryKey.Add(DicomTag.ModalitiesInStudy, queryParams["ModalitiesInStudy"]);
            queryKey.Add(DicomTag.SpecificCharacterSet, "");
			queryKey.Add(DicomTag.StudyInstanceUID, queryParams["StudyInstanceUid"]);

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
                item.PatientsBirthDate = result[DicomTag.PatientsBirthDate];
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
                ReadOnlyQueryResultCollection results = DataAccessLayer.GetIDataStoreReader().StudyQuery(queryKey);
                return results;
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
