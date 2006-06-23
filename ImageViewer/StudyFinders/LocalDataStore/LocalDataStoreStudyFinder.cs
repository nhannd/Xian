namespace ClearCanvas.ImageViewer.StudyFinders.LocalDataStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Common;
    using ClearCanvas.ImageViewer.StudyManagement;
    using ClearCanvas.Dicom;
    using ClearCanvas.DataStore;

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyFinderExtensionPoint))]
    public class LocalDataStoreStudyFinder : StudyFinder
    {
        public LocalDataStoreStudyFinder()
        {

        }

        public override string Name
        {
            get
            {
                return "My DataStore";
            }
        }

        public override StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override StudyItemList Query(QueryParameters queryParams)
        {
            QueryKey queryKey = new QueryKey();
            queryKey.Add(DicomTag.PatientId, queryParams["PatientId"]);
            queryKey.Add(DicomTag.AccessionNumber, queryParams["AccessionNumber"]);
            queryKey.Add(DicomTag.PatientsName, queryParams["PatientsName"]);
            queryKey.Add(DicomTag.StudyDate, "");
            queryKey.Add(DicomTag.StudyDescription, queryParams["StudyDescription"]);
            queryKey.Add(DicomTag.PatientsBirthDate, "");
            //queryKey.Add(DicomTag.ModalitiesInStudy, "");

            ReadOnlyQueryResultCollection results = Query(queryKey);
            if (null == results)
                return null;

            StudyItemList studyItemList = new StudyItemList();
            foreach (QueryResult result in results)
            {
                StudyItem item = new StudyItem();
                item.PatientId = result.PatientId.ToString();
                item.LastName = result.PatientsName.LastName;
                item.FirstName = result.PatientsName.FirstName;
                item.PatientsBirthDate = result[DicomTag.PatientsBirthDate];
                item.StudyDate = result.StudyDate;
                item.StudyDescription = result.StudyDescription;
                //item.ModalitiesInStudy = result.ModalitiesInStudy;
                item.AccessionNumber = result.AccessionNumber;
                item.StudyInstanceUID = result.StudyInstanceUid.ToString();
                item.StudyLoaderName = "My DataStore";

                studyItemList.Add(item);
            }

            return studyItemList;
        }

        protected ReadOnlyQueryResultCollection Query(QueryKey queryKey)
        {
            try
            {
                _connectionString.Load();
                DatabaseConnector database = new DatabaseConnector(_connectionString);
                database.SetupConnector();
                ReadOnlyQueryResultCollection results = database.StudyQuery(queryKey);
                database.TeardownConnector();
                return results;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                // TODO
                Platform.ShowMessageBox("Can't connect to data store" + e.ToString());
                return null;
            }
        }

        private ApplicationConnectionString _connectionString = new ApplicationConnectionString();
    }
}
