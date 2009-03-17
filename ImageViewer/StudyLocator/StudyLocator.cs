using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Services.Configuration;
using ClearCanvas.ImageViewer.Services.ServerTree;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyLocator
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false, ConfigurationName = "StudyLocator", Namespace = QueryNamespace.Value)]
	public class StudyLocator : IStudyRootQuery
	{
		private delegate IList<T> QueryDelegate<T>(T criteria, IStudyRootQuery query) where T : Identifier;
		private delegate string GetUidDelegate<T>(T identifier) where T : Identifier;

		private class GenericQuery<T> where T : Identifier, new()
		{
			private readonly QueryDelegate<T> _query;
			private readonly GetUidDelegate<T> _getUid;

			public GenericQuery(QueryDelegate<T> query, GetUidDelegate<T> getUid)
			{
				_query = query;
				_getUid = getUid;
			}

			public IList<T> Query(T queryCriteria)
			{
				if (queryCriteria == null)
				{
					string message = "The argument cannot be null.";
					Platform.Log(LogLevel.Error, message);
					throw new FaultException(message);
				}

				Dictionary<string, T> combinedResults = new Dictionary<string, T>();

				try
				{
					foreach (IStudyRootQuery query in GetQueryInterfaces())
					{
						try
						{
							IList<T> results = _query(queryCriteria, query);
							foreach (T result in results)
							{
								string uid = _getUid(result);
								if (!combinedResults.ContainsKey(uid))
									combinedResults[uid] = result;
							}
						}
						catch (Exception e)
						{
							QueryFailedFault fault = new QueryFailedFault();
							fault.Description = String.Format("Failed to query server {0}.", query);
							Platform.Log(LogLevel.Error, e, fault.Description);
							throw new FaultException<QueryFailedFault>(fault, fault.Description);
						}
						finally
						{
							if (query is IDisposable)
								(query as IDisposable).Dispose();
						}
					}
				}
				catch (FaultException)
				{
					throw;
				}
				catch (Exception e)
				{
					QueryFailedFault fault = new QueryFailedFault();
					fault.Description = String.Format("An unexpected error has occurred.");
					Platform.Log(LogLevel.Error, e, fault.Description);
					throw new FaultException<QueryFailedFault>(fault, fault.Description);
				}

				return new List<T>(combinedResults.Values);
			}
		}

		public StudyLocator()
		{
		}

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			QueryDelegate<StudyRootStudyIdentifier> query =
				delegate(StudyRootStudyIdentifier criteria, IStudyRootQuery studyRootQuery)
					{
						return studyRootQuery.StudyQuery(criteria);
					};

			GetUidDelegate<StudyRootStudyIdentifier> getUid =
				delegate(StudyRootStudyIdentifier result)
					{
						return result.StudyInstanceUid;
					};

			return new GenericQuery<StudyRootStudyIdentifier>(query, getUid).Query(queryCriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			QueryDelegate<SeriesIdentifier> query =
				delegate(SeriesIdentifier criteria, IStudyRootQuery studyRootQuery)
					{
						return studyRootQuery.SeriesQuery(criteria);
					};

			GetUidDelegate<SeriesIdentifier> getUid =
				delegate(SeriesIdentifier result)
					{
						return result.SeriesInstanceUid;
					};

			return new GenericQuery<SeriesIdentifier>(query, getUid).Query(queryCriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			QueryDelegate<ImageIdentifier> query =
				delegate(ImageIdentifier criteria, IStudyRootQuery studyRootQuery)
					{
						return studyRootQuery.ImageQuery(criteria);
					};

			GetUidDelegate<ImageIdentifier> getUid =
				delegate(ImageIdentifier result)
					{
						return result.SopInstanceUid;
					};

			return new GenericQuery<ImageIdentifier>(query, getUid).Query(queryCriteria);
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
			yield return serverTree.RootNode.LocalDataStoreNode;

			foreach (Server server in DefaultServers.SelectFrom(serverTree))
				yield return server;
		}
	}
}
