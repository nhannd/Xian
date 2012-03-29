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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using System.IO;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.StudyFinders.Local
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

            var oldCharacterSet = identifier.SpecificCharacterSet;
            const string utf8 = "ISO_IR 192";
            //.NET strings are unicode, so the query criteria are unicode.
            identifier.SpecificCharacterSet = utf8;
            
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
            finally
            {
                identifier.SpecificCharacterSet = oldCharacterSet;
            }

		    using (var context = new DataAccessContext())
			{
				IEnumerable<DicomAttributeCollection> results = context.GetStudyRootQuery().Query(queryCriteria);

				List<T> queryResults = new List<T>();
				foreach (DicomAttributeCollection result in results)
				{
					T queryResult = Identifier.FromDicomAttributeCollection<T>(result);

					queryResult.InstanceAvailability = "ONLINE";
				    queryResult.RetrieveAeTitle = DicomServerConfigurationHelper.AETitle;
					queryResults.Add(queryResult);
				}

				return queryResults;
			}
		}
	}
}