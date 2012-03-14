#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

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
			var impersonationContext = new ServiceClientImpersonationContext();
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
			finally
			{
				impersonationContext.Dispose();
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
