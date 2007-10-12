#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using ServiceModelEx;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal sealed class LocalDataStoreActivityMonitor : ILocalDataStoreActivityMonitorService
	{
		private class InternalSubscriptionManager : TransientSubscriptionManager<ILocalDataStoreActivityMonitorServiceCallback>
		{
			public InternalSubscriptionManager()
			{ 
			}
		}

		private static LocalDataStoreActivityMonitor _instance;

		private InternalSubscriptionManager _subscriptionManager;

		private LocalDataStoreActivityMonitor()
		{
			_subscriptionManager = new InternalSubscriptionManager();
		}

		public static LocalDataStoreActivityMonitor Instance
		{
			get
			{
				if (_instance == null)
					_instance = new LocalDataStoreActivityMonitor();

				return _instance;
			}
		}

		#region ILocalDataStoreActivityMonitorService Members

		public void Cancel(CancelProgressItemInformation information)
		{
			LocalDataStoreService.Instance.Cancel(information);
		}

		public void ClearInactive()
		{
			LocalDataStoreService.Instance.ClearInactive();
		}

		public void Refresh()
		{
			LocalDataStoreService.Instance.RepublishAll();
		}

		#endregion

		#region ISubscriptionService Members

		public void Subscribe(string eventOperation)
		{
			_subscriptionManager.Subscribe(eventOperation);
		}

		public void Unsubscribe(string eventOperation)
		{
			_subscriptionManager.Unsubscribe(eventOperation);
		}

		#endregion
	}
}
