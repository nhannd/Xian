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