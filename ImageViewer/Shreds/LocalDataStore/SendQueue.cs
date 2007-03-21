using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.Threading;

namespace ClearCanvas.ImageViewer.Shreds.LocalDataStore
{
	internal class SendQueue
	{
		private object _sendProgressItemsLock = new object();
		private List<SendProgressItem> _sendProgressItems;

		public SendQueue()
		{
		}

		public void Start()
		{ 
		
		}

		public void Stop()
		{ 
		
		}

		public void ProcessSentFileInformation(StoreScuSentFileInformation sentFileInformation)
		{ 
		
		}

		public void RepublishAll()
		{ 
		
		}
	}
}
