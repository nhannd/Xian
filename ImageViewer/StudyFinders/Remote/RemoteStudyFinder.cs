namespace ClearCanvas.ImageViewer.StudyFinders.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Common;
    using ClearCanvas.ImageViewer.StudyManagement;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyFinderExtensionPoint))]
    public class RemoteStudyFinder : StudyFinder
	{
		public RemoteStudyFinder()
		{

		}

		public override string Name
		{
			get
			{
				return "DICOM_REMOTE";
			}
		}

        public override StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams)
        {
            _selectedServer = (targetServerObject as ApplicationEntity);
            return Query(queryParams);
        }

        public override StudyItemList Query(QueryParameters queryParams)
        {
            QueryKey queryKey = new QueryKey();
            queryKey.Add(DicomTag.PatientId, queryParams["PatientId"]);
            queryKey.Add(DicomTag.AccessionNumber, queryParams["AccessionNumber"]);
            queryKey.Add(DicomTag.PatientsName, queryParams["PatientsName"]);
            queryKey.Add(DicomTag.StudyDate, "");
            queryKey.Add(DicomTag.StudyDescription, queryParams["StudyDescription"]);

            ReadOnlyQueryResultCollection results = Query(_selectedServer, queryKey);
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
                item.StudyLoaderName = this.Name;
                item.Server = _selectedServer;
                item.StudyInstanceUID = result.StudyInstanceUid.ToString();
                if (result.ContainsTag(DicomTag.NumberOfStudyRelatedInstances))
                    item.NumberOfStudyRelatedInstances = result.NumberOfStudyRelatedInstances;
                else
                    item.NumberOfStudyRelatedInstances = 0;

                studyItemList.Add(item);
            }

            return studyItemList;
        }

        protected ReadOnlyQueryResultCollection Query(ApplicationEntity server, QueryKey queryKey)
        {
            ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle("CCWORKSTN"), new ListeningPort(4000));
            using (DicomClient client = new DicomClient(me))
            {
                ReadOnlyQueryResultCollection results = client.Query(server, queryKey);
                return results;
            }
        }

        private ApplicationEntity _selectedServer;
	}
}
