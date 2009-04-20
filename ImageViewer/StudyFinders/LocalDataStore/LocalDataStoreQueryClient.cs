using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom.ServiceModel.Query;
using System.IO;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.ImageViewer.StudyLocator;

namespace ClearCanvas.ImageViewer.StudyFinders.LocalDataStore
{
	[ExtensionOf(typeof(LocalStudyRootQueryExtensionPoint))]
	internal class LocalDataStoreQueryClient : IStudyRootQuery
	{
		public LocalDataStoreQueryClient()
		{

		}

		public override string ToString()
		{
			return "<local>";
		}

		#region IStudyRootQuery Members

		public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
		{
			return Query(queryCriteria);
		}

		public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
		{
			return Query(queryCriteria);
		}

		public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
		{
			return Query(queryCriteria);
		}

		#endregion

		private static IList<T> Query<T>(T identifier) where T : Identifier, new()
		{
			try
			{
				return DoQuery(identifier);
			}
			catch (FileNotFoundException fe)
			{
				QueryFailedFault fault = new QueryFailedFault();
				fault.Description = String.Format("The local data store is not installed.");
				Platform.Log(LogLevel.Debug, fe, fault.Description);
				Platform.Log(LogLevel.Warn, fault.Description);
				throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
			catch (Exception e)
			{
				QueryFailedFault fault = new QueryFailedFault();
				fault.Description = String.Format("Unexpected error while processing study root query.");
				Platform.Log(LogLevel.Error, e, fault.Description);
				throw new FaultException<QueryFailedFault>(fault, fault.Description);
			}
		}

		private static IList<T> DoQuery<T>(T identifier) where T : Identifier, new()
		{
			if (identifier == null)
			{
				string message = "The query identifier cannot be null.";
				Platform.Log(LogLevel.Error, message);
				throw new FaultException(message);
			}

			DicomAttributeCollection queryCriteria;
			try
			{
				queryCriteria = identifier.ToDicomAttributeCollection();
			}
			catch (DicomException e)
			{
				DataValidationFault fault = new DataValidationFault();
				fault.Description = "Failed to convert contract object to DicomAttributeCollection.";
				Platform.Log(LogLevel.Error, e, fault.Description);
				throw new FaultException<DataValidationFault>(fault, fault.Description);
			}
			catch (Exception e)
			{
				DataValidationFault fault = new DataValidationFault();
				fault.Description = "Unexpected exception when converting contract object to DicomAttributeCollection.";
				Platform.Log(LogLevel.Error, e, fault.Description);
				throw new FaultException<DataValidationFault>(fault, fault.Description);
			}

			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IEnumerable<DicomAttributeCollection> results = reader.Query(queryCriteria);

				List<T> queryResults = new List<T>();
				foreach (DicomAttributeCollection result in results)
				{
					T queryResult = Identifier.FromDicomAttributeCollection<T>(result);

					queryResult.InstanceAvailability = "ONLINE";
					queryResult.RetrieveAeTitle = ServerTree.GetClientAETitle();
					queryResults.Add(queryResult);
				}

				return queryResults;
			}
		}
	}
}