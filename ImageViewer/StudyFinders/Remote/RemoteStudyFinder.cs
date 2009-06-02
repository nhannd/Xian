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

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyFinders.Remote
{
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
			ApplicationEntity selectedServer = (ApplicationEntity)targetServer;

			DicomAttributeCollection requestCollection = new DicomAttributeCollection();

			requestCollection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			requestCollection[DicomTags.StudyInstanceUid].SetStringValue("");

			requestCollection[DicomTags.PatientId].SetStringValue(queryParams["PatientId"]);
			requestCollection[DicomTags.AccessionNumber].SetStringValue(queryParams["AccessionNumber"]);
			requestCollection[DicomTags.PatientsName].SetStringValue(queryParams["PatientsName"]);
			requestCollection[DicomTags.StudyDate].SetStringValue(queryParams["StudyDate"]);
			requestCollection[DicomTags.StudyTime].SetStringValue("");
			requestCollection[DicomTags.StudyDescription].SetStringValue(queryParams["StudyDescription"]);
			requestCollection[DicomTags.PatientsBirthDate].SetStringValue("");
			requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(queryParams["ModalitiesInStudy"]);
			requestCollection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");
			requestCollection[DicomTags.SpecificCharacterSet].SetStringValue("");
			requestCollection[DicomTags.InstanceAvailability].SetStringValue("");

			IList<DicomAttributeCollection> results = Query(selectedServer, requestCollection);
			
			StudyItemList studyItemList = new StudyItemList();
			foreach (DicomAttributeCollection result in results)
			{
				StudyItem item = new StudyItem(result[DicomTags.StudyInstanceUid].GetString(0, ""), selectedServer, this.Name);

				//TODO: add DicomField attributes to the StudyItem class (implement typeconverter for PersonName class).
				item.PatientsBirthDate = result[DicomTags.PatientsBirthDate].GetString(0, "");
				item.AccessionNumber = result[DicomTags.AccessionNumber].GetString(0, "");
				item.StudyDescription = result[DicomTags.StudyDescription].GetString(0, "");
				item.StudyDate = result[DicomTags.StudyDate].GetString(0, "");
				item.StudyTime = result[DicomTags.StudyTime].GetString(0, "");
				item.PatientId = result[DicomTags.PatientId].GetString(0, "");
				item.PatientsName = new PersonName(result[DicomTags.PatientsName].GetString(0, ""));
				item.ModalitiesInStudy = result[DicomTags.ModalitiesInStudy].ToString();
				item.NumberOfStudyRelatedInstances = result[DicomTags.NumberOfStudyRelatedInstances].GetUInt32(0, 0);
				item.SpecificCharacterSet = result.SpecificCharacterSet;
				item.InstanceAvailability = result[DicomTags.InstanceAvailability].GetString(0, "");
				if (String.IsNullOrEmpty(item.InstanceAvailability))
					item.InstanceAvailability = "ONLINE";

				studyItemList.Add(item);
			}

			AuditedInstances queriedInstances = new AuditedInstances();
			studyItemList.ForEach(delegate(StudyItem study) { queriedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUID); });
			AuditHelper.LogQueryStudies(selectedServer.AETitle, selectedServer.Host, queriedInstances, EventSource.CurrentUser, EventResult.Success);

			return studyItemList;
        }

		protected IList<DicomAttributeCollection> Query(ApplicationEntity server, DicomAttributeCollection requestCollection)
		{
			//special case code for ModalitiesInStudy.  An IStudyFinder must accept a multi-valued
			//string for ModalitiesInStudy (e.g. "MR\\CT") and process it appropriately for the 
			//datasource that is being queried.  In this case (Dicom) does not allow multiple
			//query keys, so we have to execute one query per modality specified in the 
			//ModalitiesInStudy query item.

			List<string> modalityFilters = new List<string>();
			if (requestCollection.Contains(DicomTags.ModalitiesInStudy))
			{
				string modalityFilterString = requestCollection[DicomTags.ModalitiesInStudy].ToString();
				if (!String.IsNullOrEmpty(modalityFilterString))
					modalityFilters.AddRange(modalityFilterString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

				if (modalityFilters.Count == 0)
					modalityFilters.Add(""); //make sure there is at least one filter, the default.
			}

			SortedList<string, DicomAttributeCollection> resultsByStudy = new SortedList<string, DicomAttributeCollection>();

			string combinedFilter = requestCollection[DicomTags.ModalitiesInStudy];

			try 
			{
				foreach (string modalityFilter in modalityFilters) 
				{
					using (StudyRootFindScu scu = new StudyRootFindScu())
					{
						requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(modalityFilter);

						string myAE = ServerTree.GetClientAETitle();

						IList<DicomAttributeCollection> results = scu.Find(ServerTree.GetClientAETitle(),
							server.AETitle, server.Host, server.Port,
							requestCollection);

						scu.Join(new TimeSpan(0, 0, 0, 0, 1000));

						if(scu.Status == ScuOperationStatus.Canceled)
						{
							String message = String.Format(SR.MessageFormatRemoteServerCancelledFind, 
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.ConnectFailed)
						{
							String message = String.Format(SR.MessageFormatConnectionFailed,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						else if (scu.Status == ScuOperationStatus.AssociationRejected)
						{
							String message = String.Format(SR.MessageFormatAssociationRejected,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.Failed)
						{
							String message = String.Format(SR.MessageFormatQueryOperationFailed,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.TimeoutExpired)
						{
							String message = String.Format(SR.MessageFormatConnectTimeoutExpired,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.NetworkError)
						{
							throw new DicomException(SR.MessageUnexpectedNetworkError);
						}
						if (scu.Status == ScuOperationStatus.UnexpectedMessage)
						{
							throw new DicomException(SR.MessageUnexpectedMessage);
						}

						//if this function returns true, it means that studies came back whose 
						//modalities did not match the filter, meaning that filtering on
						//ModalitiesInStudy is not supported by that server.
						if (FilterResultsByModality(results, resultsByStudy, modalityFilter))
							break;
					}
				}

				return new List<DicomAttributeCollection>(resultsByStudy.Values);
			}
			finally
			{
				//for consistencies sake, put the original filter back.
				requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(combinedFilter);
			}
		}

		protected static bool FilterResultsByModality(IList<DicomAttributeCollection> results, IDictionary<string, DicomAttributeCollection> resultsByStudy, string modalityFilter)
		{
			//if this particular filter is a wildcard filter, we won't try to be smart about running extra queries.
			bool isWildCardQuery = (modalityFilter.IndexOfAny(new char[] { '?', '*' }) >= 0);

			//if the filter is "", then everything is a match.
			bool everythingMatches = String.IsNullOrEmpty(modalityFilter);

			foreach (DicomAttributeCollection result in results)
			{
				string studyInstanceUid = result[DicomTags.StudyInstanceUid].ToString();
				if (resultsByStudy.ContainsKey(studyInstanceUid))
					continue;

				bool matchesFilter = true;

				if (!everythingMatches)
				{
					//the server does not support this optional tag at all.
					if (!result.Contains(DicomTags.ModalitiesInStudy))
					{
						everythingMatches = true;
					}
					else if (!isWildCardQuery)
					{
						string returnedModalities = result[DicomTags.ModalitiesInStudy].ToString();
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
					resultsByStudy[studyInstanceUid] = result;
			}

			return everythingMatches;
		}
	}
}
