#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Common.Auditing;
using System.Net;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class FindScpExtension : ScpExtension
	{
		public FindScpExtension()
			: base(GetSupportedSops())
		{
		}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
		    var sop = new SupportedSop
		                  {
		                      SopClass = SopClass.StudyRootQueryRetrieveInformationModelFind
		                  };
		    sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
			sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
			yield return sop;
		}

		private static string GetRemoteHostName(AssociationParameters association)
		{
			string remoteHostName = null;
			try
			{
				if (association.RemoteEndPoint != null)
				{
					try
					{
						IPHostEntry entry = Dns.GetHostEntry(association.RemoteEndPoint.Address);
						remoteHostName = entry.HostName;
					}
					catch
					{
						remoteHostName = association.RemoteEndPoint.Address.ToString();
					}
				}
			}
			catch (Exception e)
			{
				remoteHostName = null;
				Platform.Log(LogLevel.Warn, e, "Unable to resolve remote host name for auditing.");
			}

			return remoteHostName;
		}

        public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "").Trim();

			if (message.AffectedSopClassUid.Equals(SopClass.StudyRootQueryRetrieveInformationModelFindUid))
			{
				try
				{
					using (var context = new DataAccessContext())
					{
						IEnumerable<DicomAttributeCollection> results = context.GetStudyStoreQuery().Query(message.DataSet);
						foreach (DicomAttributeCollection result in results)
						{
                            const string utf8 = "ISO_IR 192";
                            if (DicomServerSettings.Instance.QueryResponsesInUtf8)
                                ChangeCharacterSet(result, utf8);

                            var response = new DicomMessage(null, result);

						    //Add these to each response.
							message.DataSet[DicomTags.RetrieveAeTitle].SetStringValue(Context.AETitle);
							message.DataSet[DicomTags.InstanceAvailability].SetStringValue("ONLINE");

							response.DataSet[DicomTags.QueryRetrieveLevel].SetStringValue(level);
							server.SendCFindResponse(presentationID, message.MessageId, response,
													 DicomStatuses.Pending);
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e, "Unexpected exception when processing FIND request.");

					try
					{
						var errorResponse = new DicomMessage();
						server.SendCFindResponse(presentationID, message.MessageId, errorResponse,
						                         DicomStatuses.QueryRetrieveUnableToProcess);

						return true;
					}
					finally
					{
						AuditHelper.LogQueryReceived(association.CallingAE, GetRemoteHostName(association), EventResult.SeriousFailure,
						                             message.AffectedSopClassUid, message.DataSet);
					}
				}

				try
				{
					var finalResponse = new DicomMessage();
					server.SendCFindResponse(presentationID, message.MessageId, finalResponse, DicomStatuses.Success);

					AuditHelper.LogQueryReceived(association.CallingAE, GetRemoteHostName(association), EventResult.Success,
					                             message.AffectedSopClassUid, message.DataSet);
					return true;
				}
				catch
				{
					AuditHelper.LogQueryReceived(association.CallingAE, GetRemoteHostName(association), EventResult.SeriousFailure,
					                             message.AffectedSopClassUid, message.DataSet);
					throw;
				}
			}

			try
			{
				Platform.Log(LogLevel.Error, "Unexpected Study Root Query/Retrieve level: {0}", level);
				server.SendCFindResponse(presentationID, message.MessageId, new DicomMessage(),
										 DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
				return true;
			}
			finally
			{
				AuditHelper.LogQueryReceived(association.CallingAE, GetRemoteHostName(association), EventResult.SeriousFailure,
				                             message.AffectedSopClassUid, message.DataSet);
			}
		}

	    // TODO (CR Mar 2012): Hack for now to make sure character set is consistent in root and all sequences.
        private static void ChangeCharacterSet(DicomAttributeCollection attributes, string characterSet)
	    {
            ChangeCharacterSet(attributes, characterSet, true);
	    }

        private static void ChangeCharacterSet(DicomAttributeCollection attributes, string characterSet, bool isRoot)
	    {
            attributes.SpecificCharacterSet = characterSet;
            if (isRoot)
                attributes[DicomTags.SpecificCharacterSet].SetStringValue(characterSet);

            foreach (var attribute in attributes.OfType<DicomAttributeSQ>())
            {
                var items = attribute.Values as DicomSequenceItem[];
                if (items == null || items.Length == 0)
                    continue;

                foreach (var item in items)
                    ChangeCharacterSet(item, characterSet, false);
            }
	    }
    }
}
