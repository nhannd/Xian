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
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.LocalDataStore
{
	public class LocalDataStoreServiceClient : ClientBase<ILocalDataStoreService>, ILocalDataStoreService
	{
		public LocalDataStoreServiceClient()
		{
		}

		#region ILocalDataStoreService Members

		public void RetrieveStarted(RetrieveStudyInformation information)
		{
			base.Channel.RetrieveStarted(information);
		}

		public void FileReceived(StoreScpReceivedFileInformation receivedFileInformation)
		{
			base.Channel.FileReceived(receivedFileInformation);
		}

		public void ReceiveError(ReceiveErrorInformation errorInformation)
		{
			base.Channel.ReceiveError(errorInformation);
		}

		public void SendStarted(SendStudyInformation information)
		{
			base.Channel.SendStarted(information);
		}

		public void FileSent(StoreScuSentFileInformation sentFileInformation)
		{
			base.Channel.FileSent(sentFileInformation);
		}

		public void SendError(SendErrorInformation errorInformation)
		{
			base.Channel.SendError(errorInformation);
		}

		public LocalDataStoreServiceConfiguration GetConfiguration()
		{
			return base.Channel.GetConfiguration();
		}

		public void DeleteInstances(DeleteInstancesRequest request)
		{
			base.Channel.DeleteInstances(request);
		}

		public Guid Import(FileImportRequest request)
		{
			return base.Channel.Import(request);
		}

		public void Reindex()
		{
			base.Channel.Reindex();
		}

		#endregion
	}
}