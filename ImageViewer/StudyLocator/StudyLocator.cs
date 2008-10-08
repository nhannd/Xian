using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.ServerTree;
using System.ServiceModel;
using ClearCanvas.ImageViewer.Services.StudyLocator;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyLocator
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConfigurationName = "StudyLocator", Namespace = StudyLocatorNamespace.Value)]
	public class StudyLocator : IStudyLocator
	{
		private delegate IList<T> QueryDelegate<T>(T criteria, IStudyRootQuery query) where T : Identifier;
		private delegate string GetUidDelegate<T>(T identifier) where T : Identifier;

		#region Static Helpers

		private static IList<StudyRootStudyIdentifier> DoStudyQuery(StudyRootStudyIdentifier criteria, IStudyRootQuery query)
		{
			return query.StudyQuery(criteria);
		}
		private static IList<SeriesIdentifier> DoSeriesQuery(SeriesIdentifier criteria, IStudyRootQuery query)
		{
			return query.SeriesQuery(criteria);
		}
		private static IList<ImageIdentifier> DoImageQuery(ImageIdentifier criteria, IStudyRootQuery query)
		{
			return query.ImageQuery(criteria);
		}

		private static string GetStudyUid(StudyRootStudyIdentifier identifier)
		{
			return identifier.StudyInstanceUid;
		}
		private static string GetSeriesUid(SeriesIdentifier identifier)
		{
			return identifier.SeriesInstanceUid;
		}
		private static string GetImageUid(ImageIdentifier identifier)
		{
			return identifier.SopInstanceUid;
		}

		private static IList<T> Query<T>(T queryQriteria, QueryDelegate<T> doQuery, GetUidDelegate<T> getUid) where T : Identifier
		{
			if (queryQriteria == null)
			{
				string message = "The argument cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			Dictionary<string, T> combinedResults = new Dictionary<string, T>();

			try
			{
				bool anySucceeded = false;
				foreach (IStudyRootQuery query in GetQueryInterfaces())
				{
					try
					{
						IList<T> results = doQuery(queryQriteria, query);
						anySucceeded = true;
						foreach (T result in results)
						{
							string uid = getUid(result);
							if (!combinedResults.ContainsKey(uid))
								combinedResults[uid] = result;
						}
					}
					catch (Exception e)
					{
						string message = string.Format("Query failed (server: {0}).", query);
						Platform.Log(LogLevel.Error, e, message);
					}
					finally
					{
						if (query is IDisposable)
							(query as IDisposable).Dispose();
					}
				}

				if (!anySucceeded)
				{
					QueryFailedFault fault = new QueryFailedFault();
					fault.Description = String.Format("Failed to query any of the default servers.");
					Platform.Log(LogLevel.Error, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}
			}
			catch(FaultException)
			{
				throw;
			}
			catch(Exception	e)
			{
				QueryFailedFault fault = new QueryFailedFault();
				fault.Description = String.Format("An unexpected error has occurred.");
				Platform.Log(LogLevel.Error, e, fault.Description);
				throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}

			return new List<T>(combinedResults.Values);
		}

		#endregion

		#region IStudyLocator Members

		public IList<StudyRootStudyIdentifier> FindByStudyInstanceUid(string[] studyInstanceUids)
		{
			if(studyInstanceUids == null || studyInstanceUids.Length == 0)
				throw new FaultException("The argument cannot be null or empty.");

			string studyUids = DicomStringHelper.GetDicomStringArray(studyInstanceUids);
			if (String.IsNullOrEmpty(studyUids))
				throw new FaultException("The argument must have valid study instance uid values.");

			StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
			identifier.StudyInstanceUid = studyUids;
			return StudyQuery(identifier);
		}

		public IList<StudyRootStudyIdentifier> FindByAccessionNumber(string accessionNumber)
		{
			if (String.IsNullOrEmpty(accessionNumber))
				throw new FaultException("The argument cannot be null or empty.");
				
			if (accessionNumber.Contains("\\") || accessionNumber.Contains("*") || accessionNumber.Contains("?"))
			{
				DataValidationFault fault = new DataValidationFault();
				fault.Description = "The accession number cannot contain any wildcard characters, or multiple values.";
				Platform.Log(LogLevel.Error, fault.Description);
				throw new FaultException<DataValidationFault>(fault, fault.Description);
			}

			StudyRootStudyIdentifier identifier = new StudyRootStudyIdentifier();
			identifier.AccessionNumber = accessionNumber;
			return StudyQuery(identifier);
		}

		#endregion

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryQriteria)
		{
			return Query(queryQriteria, DoStudyQuery, GetStudyUid);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryQriteria)
		{
			return Query(queryQriteria, DoSeriesQuery, GetSeriesUid);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryQriteria)
		{
			return Query(queryQriteria, DoImageQuery, GetImageUid);
		}

		#endregion

		private static IEnumerable<IStudyRootQuery> GetQueryInterfaces()
		{
			yield return new LocalDataStoreQueryClient();

			string localAE = ServerTree.GetClientAETitle();
			foreach (IServerTreeNode server in GetDefaultServers())
			{
				if (server.IsServer)
				{
					Server dicomServer = (Server) server;
					yield return new StudyRootQueryClient(localAE, dicomServer.AETitle, dicomServer.Host, dicomServer.Port);
				}
			}
		}

		private static IEnumerable<IServerTreeNode> GetDefaultServers()
		{
			ServerTree serverTree = new ServerTree();
			return serverTree.FindDefaultServers(serverTree.RootNode.ServerGroupNode);
		}
	}
}
