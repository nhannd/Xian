namespace ClearCanvas.ImageViewer.StudyFinders.Remote
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Common;
    using ClearCanvas.ImageViewer.StudyManagement;
    using ClearCanvas.Dicom;
    using ClearCanvas.Dicom.Network;
	using ClearCanvas.ImageViewer.Configuration;

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyFinderExtensionPoint))]
    public class RemoteStudyFinder : IStudyFinder
	{
		public RemoteStudyFinder()
		{

		}

		public string Name
		{
			get
			{
				return "DICOM_REMOTE";
			}
		}

        public StudyItemList Query(QueryParameters queryParams, object targetServer)
        {
			_selectedServer = (ApplicationEntity)targetServer;

			QueryKey queryKey = new QueryKey();
            queryKey.Add(DicomTag.PatientId, queryParams["PatientId"]);
            queryKey.Add(DicomTag.AccessionNumber, queryParams["AccessionNumber"]);
            queryKey.Add(DicomTag.PatientsName, queryParams["PatientsName"]);
            queryKey.Add(DicomTag.StudyDate, queryParams["StudyDate"]);
            queryKey.Add(DicomTag.StudyDescription, queryParams["StudyDescription"]);
            queryKey.Add(DicomTag.PatientsBirthDate, "");
            queryKey.Add(DicomTag.ModalitiesInStudy, queryParams["ModalitiesInStudy"]);
            queryKey.Add(DicomTag.SpecificCharacterSet, "");

            ReadOnlyQueryResultCollection results = Query(_selectedServer, queryKey);
            if (null == results)
                return null;

            StudyItemList studyItemList = new StudyItemList();
            foreach (QueryResult result in results)
            {
                StudyItem item = new StudyItem();
                item.SpecificCharacterSet = result.SpecificCharacterSet;
                item.PatientId = result.PatientId.ToString();

                if (item.SpecificCharacterSet == String.Empty)
                    item.PatientsName = result.PatientsName;
                else
                    item.PatientsName = new PersonName(SpecificCharacterSetParser.Parse(item.SpecificCharacterSet, result.PatientsName));

                item.PatientsBirthDate = result[DicomTag.PatientsBirthDate];
                item.StudyDate = result.StudyDate;
                item.StudyDescription = result.StudyDescription;
                item.ModalitiesInStudy = result.ModalitiesInStudy;
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
			ApplicationEntity me = new ApplicationEntity(	new HostName(DicomServerConfigurationHelper.Host), 
															new AETitle(DicomServerConfigurationHelper.AETitle), 
															new ListeningPort(DicomServerConfigurationHelper.Port));

			//special case code for ModalitiesInStudy.  An IStudyFinder must accept a multi-valued
			//string for ModalitiesInStudy (e.g. "MR\\CT") and process it appropriately for the 
			//datasource that is being queried.  In this case (Dicom) does not allow multiple
			//query keys, so we have to execute one query per modality specified in the 
			//ModalitiesInStudy query item.

			List<string> modalityFilters = new List<string>();
			if (queryKey.ContainsTag(DicomTag.ModalitiesInStudy))
			{
				string modalityFilterString = queryKey[DicomTag.ModalitiesInStudy].ToString();
				if (!String.IsNullOrEmpty(modalityFilterString))
					modalityFilters.AddRange(modalityFilterString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

				if (modalityFilters.Count == 0)
					modalityFilters.Add(""); //make sure there is at least one filter, the default.
			}

			SortedList<string, QueryResult> resultsByStudy = new SortedList<string, QueryResult>();

			string combinedFilter = queryKey[DicomTag.ModalitiesInStudy];

			try
			{
				foreach (string modalityFilter in modalityFilters)
				{
					queryKey[DicomTag.ModalitiesInStudy] = modalityFilter;

					using (DicomClient client = new DicomClient(me))
					{
						ReadOnlyQueryResultCollection results = client.Query(server, queryKey);

						//if this function returns true, it means that studies came back whose 
						//modalities did not match the filter, meaning that filtering on
						//ModalitiesInStudy is not supported by that server.
						if (FilterResultsByModality(results, resultsByStudy, modalityFilter))
							break;
					}
				}

				return new ReadOnlyQueryResultCollection(resultsByStudy.Values);
			}
			catch
			{
				throw;
			}
			finally
			{
				//for consistencies sake, put the original filter back.
				queryKey[DicomTag.ModalitiesInStudy] = combinedFilter;
			}

        }

		protected bool FilterResultsByModality(ReadOnlyQueryResultCollection results, IDictionary<string, QueryResult> resultsByStudy, string modalityFilter)
		{
			//if this particular filter is a wildcard filter, we won't try to be smart about running extra queries.
			bool isWildCardQuery = (modalityFilter.IndexOfAny(new char[] { '?', '*'}) >= 0);

			//if the filter is "", then everything is a match.
			bool everythingMatches = String.IsNullOrEmpty(modalityFilter);
			
			foreach (QueryResult result in results)
			{
				if (resultsByStudy.ContainsKey(result.StudyInstanceUid))
					continue;

				bool matchesFilter = true;

				if (!everythingMatches)
				{
					//the server does not support this optional tag at all.
					if (!result.ContainsTag(DicomTag.ModalitiesInStudy))
					{
						everythingMatches = true;
					}
					else if (!isWildCardQuery)
					{
						string returnedModalities = result.ModalitiesInStudy;
						string[] returnedModalitiesArray = returnedModalities.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

						if (returnedModalitiesArray == null || returnedModalitiesArray.Length == 0)
						{
							matchesFilter = false;
						}
						else
						{
							matchesFilter = false;
							foreach (string returnedModality in returnedModalitiesArray)
							{
								if (returnedModality == modalityFilter)
								{
									matchesFilter = true;
									break;
								}
							}

							// if we get back any studies that do not contain the modality specified in the filter,
							// then that means the server does not support queries on ModalitiesInStudy, so we may
							// as well stop querying because we already have all the results.
							if (!matchesFilter)
								everythingMatches = true;
						}
					}
					else
					{ 
						//!!We don't actually use wildcard queries for modality, so this is not critical right now.  When C-FIND is written
						//!!a method for post-filtering with wildcards will need to be determined.  At which point this can be completed as well.
					}
				}

				if (matchesFilter)
					resultsByStudy[result.StudyInstanceUid] = result;
			}

			return everythingMatches;
		}

        private ApplicationEntity _selectedServer;
	}
}
