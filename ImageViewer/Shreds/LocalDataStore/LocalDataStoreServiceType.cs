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
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	[Serializable]
	internal class LocalDataStoreException : Exception
	{
		public LocalDataStoreException(string message)
			: base(message)
		{
		}

		protected LocalDataStoreException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ 
		}
	}

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class LocalDataStoreServiceType : ILocalDataStoreService
	{
		public LocalDataStoreServiceType()
		{
		}

		#region ILocalDataStoreService Members

		public void RetrieveStarted(RetrieveStudyInformation information)
		{
			try
			{
				LocalDataStoreService.Instance.RetrieveStarted(information);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorProcessingRetrieveStarted;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void FileReceived(StoreScpReceivedFileInformation filesReceivedInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FileReceived(filesReceivedInformation);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = String.Format(SR.ExceptionErrorProcessingReceivedFile, filesReceivedInformation.FileName);
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void ReceiveError(ReceiveErrorInformation errorInformation)
		{
			try
			{
				LocalDataStoreService.Instance.ReceiveError(errorInformation);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorProcessingReceiveError;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void SendStarted(SendStudyInformation information)
		{
			try
			{
				LocalDataStoreService.Instance.SendStarted(information);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e); 
				string message = SR.ExceptionErrorProcessingSendStarted;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			try
			{
				LocalDataStoreService.Instance.FileSent(sentFileInformation);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = String.Format(SR.ExceptionErrorProcessingSentFile, sentFileInformation.FileName);
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void SendError(SendErrorInformation errorInformation)
		{
			try
			{
				LocalDataStoreService.Instance.SendError(errorInformation);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorProcessingSendError;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public LocalDataStoreServiceConfiguration GetConfiguration()
		{
			try
			{
				return LocalDataStoreService.Instance.GetConfiguration();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorRetrievingLocalDataStoreConfiguration;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void DeleteInstances(DeleteInstancesRequest request)
		{
			try
			{
				LocalDataStoreService.Instance.DeleteInstances(request);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorProcessingDeleteRequest;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public Guid Import(FileImportRequest request)
		{
			try
			{
				return LocalDataStoreService.Instance.Import(request);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				string message = SR.ExceptionErrorProcessingImportRequest;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		public void Reindex()
		{
			try
			{
				LocalDataStoreService.Instance.Reindex();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e); 
				string message = SR.ExceptionErrorProcessingReindexRequest;
				string exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
				throw new LocalDataStoreException(exceptionMessage);
			}
		}

		#endregion
	}
}
