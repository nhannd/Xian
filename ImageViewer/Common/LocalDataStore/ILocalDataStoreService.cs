#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.LocalDataStore
{
	/// <summary>
	/// The Local Data Store service provides a central place for performing common functions on
	/// the Local Data Store (database).  Importing of files as well as reindexing the database are 
	/// functions provided by this service.  The service also provides event publishing/notification
	/// for Dicom server(s) wishing to either import received files or notify clients that files from
	/// the Local Data Store have been sent to another server.
	/// </summary>
	/// <remarks>
	/// For reasons of simplification, the local datastore services do not adhere strictly to 
	/// the publish-subscribe pattern where there are separate services for publish-only and
	/// subscription/callback functionality.
	/// </remarks>
	[ServiceContract(ConfigurationName = "ILocalDataStoreService")]
	public interface ILocalDataStoreService
	{
		#region Receive/Retrieve event publishing
		
		/// <summary>
		/// Notifies the Local Data Store service that a retrieval has been initiated.  
		/// The Local Data Store service is only interested in the study level information
		/// regardless of the retrieve level, which is why the <see cref="RetrieveStudyInformation"/> 
		/// object is passed as a parameter.
		/// </summary>
		/// <param name="information">information about the study retrieval</param>
		[OperationContract]
		void RetrieveStarted(RetrieveStudyInformation information);

		/// <summary>
		/// Notifies the Local Data Store service that a file has been received.
		/// </summary>
		/// <param name="receivedFileInformation">the relevant information about the received file.</param>
		[OperationContract]
		void FileReceived(StoreScpReceivedFileInformation receivedFileInformation);

		/// <summary>
		/// Notifies the Local Data Store service that a *terminal* error has occurred for a receive/retrieve operation.
		/// Meant only for reporting errors that have occurred relating to the overall 'receive' (or retrieve) operation.
		/// </summary>
		/// <param name="errorInformation">error information for the receive operation</param>
		[OperationContract]
		void ReceiveError(ReceiveErrorInformation errorInformation);

		#endregion

		#region Send event publishing

		/// <summary>
		/// Notifies the Local Data Store service that some Dicom Instances (studies/series/images) specified by Instance Uid
		/// are about to be sent to another server.  The Local Data Store service is only interested in the study level information
		/// about those instances, though, which is why the <see cref="SendStudyInformation"/> object is passed as a parameter.
		/// </summary>
		/// <param name="information">study level information about the instances being sent</param>
		[OperationContract]
		void SendStarted(SendStudyInformation information);

		/// <summary>
		/// Notifies the Local Data Store service that a file has been sent to another AE.
		/// </summary>
		/// <param name="sentFileInformation">relevant information about the sent file</param>
		[OperationContract]
		void FileSent(StoreScuSentFileInformation sentFileInformation);

		/// <summary>
		/// Notifies the Local Data Store service that a *terminal* error has occurred for a send operation.
		/// This method is meant only for reporting errors related to the overall 'send' operation.
		/// </summary>
		/// <param name="errorInformation">error information for the send operation</param>
		[OperationContract]
		void SendError(SendErrorInformation errorInformation);

		#endregion

		/// <summary>
		/// Gets the Local Data Store service configuration parameters.
		/// </summary>
		/// <returns>the Local Data Store configuration parameters.</returns>
		[OperationContract]
		LocalDataStoreServiceConfiguration GetConfiguration();

		/// <summary>
		/// Requests that a collection of instances (study, series, sop) be deleted from the Local Data Store.
		/// </summary>
		/// <param name="request">the deletion request information</param>
		[OperationContract]
		void DeleteInstances(DeleteInstancesRequest request);

		/// <summary>
		/// Requests that a set of files/folders be imported into the Local Data Store.
		/// </summary>
		/// <param name="request">the request information</param>
		/// <returns></returns>
		[OperationContract]
		Guid Import(FileImportRequest request);

		/// <summary>
		/// Requests that a reindex operation be performed on the Local Data Store
		/// </summary>
		[OperationContract]
		void Reindex();
	}
}
