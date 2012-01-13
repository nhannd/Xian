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
using ClearCanvas.Dicom.ServiceModel.Query;
using System.ServiceModel;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Configuration
{
    //Configuration is not the right place for this, but it was in a lonely plugin
    //all by itself, which seems ridiculous.  Badly need to do some code reorg and refactoring.

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

			    var results = new List<T>();
				try
				{
					foreach (IStudyRootQuery query in DefaultServers.GetQueryInterfaces(true))
					{
						try
						{
							IList<T> r = _query(queryCriteria, query);
                            results.AddRange(r);
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

			    return results;
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
	}
}
