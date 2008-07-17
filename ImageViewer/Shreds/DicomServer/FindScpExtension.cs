using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.Network;
using ClearCanvas.DicomServices;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class FindScpExtension : ScpExtension, IDicomScp<IDicomServerContext>
	{
		public FindScpExtension()
			: base(GetSupportedSops())
		{
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			SupportedSop sop = new SupportedSop();
			sop.SopClass = SopClass.StudyRootQueryRetrieveInformationModelFind;
			sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

		private static QueryKey BuildQueryKey(DicomAttributeCollection attributes)
		{
			QueryKey key = new QueryKey();

			foreach (DicomAttribute attribute in attributes)
			{
				if (!attribute.IsNull && !attribute.IsEmpty)
					key[attribute.Tag] = attribute.ToString();
			}

			return key;
		}

		private DicomMessage BuildStudyQueryResponse(QueryResult result)
		{
			DicomMessage message = new DicomMessage();

			foreach (KeyValuePair<DicomTagPath, string> pair in result)
			{
				if (!String.IsNullOrEmpty(pair.Value) && pair.Key.TagsInPath.Count > 0)
				{
					if (pair.Key.TagsInPath[0].TagValue == DicomTags.SpecificCharacterSet)
						continue;

					bool clear = false;
					DicomAttributeCollection dataset = message.DataSet;
					for (int i = 0; i < pair.Key.TagsInPath.Count - 1; ++i)
					{
						DicomTag tag = pair.Key.TagsInPath[i];
						DicomAttributeSQ sequence = dataset[tag] as DicomAttributeSQ;
						if (sequence == null)
						{
							clear = true;
							break;
						}

					}

					if (clear)
						dataset[pair.Key.TagsInPath[0]].SetNullValue();
					else
						dataset[pair.Key.TagsInPath[pair.Key.TagsInPath.Count - 1]].SetStringValue(pair.Value);
				}
			}

			message.DataSet[DicomTags.RetrieveAeTitle].SetStringValue(Context.AETitle);
			message.DataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

			if (!String.IsNullOrEmpty(result.SpecificCharacterSet))
				message.DataSet[DicomTags.SpecificCharacterSet].SetStringValue(result.SpecificCharacterSet);

			return message;
		}
		
		private bool OnReceiveStudyLevelQuery(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message)
		{
			try
			{
				using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
				{
					ReadOnlyQueryResultCollection results = reader.StudyQuery(BuildQueryKey(message.DataSet));
					foreach (QueryResult result in results)
					{
						DicomMessage response = BuildStudyQueryResponse(result);
						response.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
						server.SendCFindResponse(presentationID, message.MessageId, response,
												 DicomStatuses.Pending);
					}
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");
				DicomMessage errorResponse = new DicomMessage();
				server.SendCFindResponse(presentationID, message.MessageId, errorResponse,
											 DicomStatuses.ProcessingFailure);

				return true;
			}

			DicomMessage finalResponse = new DicomMessage();
			server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);

			return true;
		}

		private bool OnReceiveSeriesLevelQuery(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message)
		{
			//TODO: series query not yet implemented.

			server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
						 DicomStatuses.QueryRetrieveUnableToProcess);
			
			return true;
		}

		private bool OnReceiveImageLevelQuery(Dicom.Network.DicomServer server, byte presentationID, DicomMessage message)
		{
			//TODO: image query not yet implemented.

			server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
						 DicomStatuses.QueryRetrieveUnableToProcess);

			return true;
		}

		public override bool OnReceiveRequest(Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "").Trim();

			if (message.AffectedSopClassUid.Equals(SopClass.StudyRootQueryRetrieveInformationModelFindUid))
			{
				if (level.Equals("STUDY"))
				{
					return OnReceiveStudyLevelQuery(server, presentationID, message);
				}
				else if (level.Equals("SERIES"))
				{
					return OnReceiveSeriesLevelQuery(server, presentationID, message);
				}
				else if (level.Equals("IMAGE"))
				{
					return OnReceiveImageLevelQuery(server, presentationID, message);
				}
			}

			Platform.Log(LogLevel.Error, "Unexpected Study Root Query/Retrieve level: {0}", level);
			server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
									 DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
			return true;
		}
	}
}
