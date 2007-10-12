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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.OffisNetwork
{
	public class DicomEventArgs : EventArgs
	{
		private IntPtr _interopCallbackInfoPointer;

		public DicomEventArgs(IntPtr interopCallbackInfoPointer)
		{
			_interopCallbackInfoPointer = interopCallbackInfoPointer;
		}

		public IntPtr CallbackInfoPointer
		{
			get { return _interopCallbackInfoPointer; }
			set { _interopCallbackInfoPointer = value; }
		}
	}
	
	public class DicomEventManager : IDisposable
    {
		private static DicomEventManager _instance;

		private FindScpCallbackHelper _findScpCallbackHelper;
		private StoreScpCallbackHelper _storeScpCallbackHelper;
		private MoveScpCallbackHelper _moveScpCallbackHelper;

		private StoreScuCallbackHelper _storeScuCallbackHelper;
		private QueryCallbackHelper _queryCallbackHelper;
		private RetrieveCallbackHelper _retrieveCallbackHelper;

		/// <summary>
		/// Fires when a C-FIND query has been received
		/// </summary>
		private event EventHandler<DicomEventArgs> _findScpEvent;
		private object _findScpEventLock = new object();
		/// <summary>
		/// Fires when a C-FIND query has asked for an update
		/// </summary>
		private event EventHandler<DicomEventArgs> _findScpProgressEvent;
		private object _findScpProgressEventLock = new object();
		/// <summary>
		/// Fires when a new SOP Instance has arrived and is starting to be written to the local filesystem.
		/// </summary>
		private event EventHandler<DicomEventArgs> _storeScpBeginEvent;
		private object _storeScpBeginEventLock = new object();
		/// <summary>
		/// Fires when more data of the new SOP Instance has arrived
		/// </summary>
		private event EventHandler<DicomEventArgs> _storeScpProgressEvent;
		private object _storeScpProgressEventLock = new object();
		/// <summary>
		/// Fires when a new SOP Instance has been successfully written to the local filesystem.
		/// </summary>
		private event EventHandler<DicomEventArgs> _storeScpEndEvent;
		private object _storeScpEndEventLock = new object();
		/// <summary>
		/// Fires when a C-MOVE query has been received
		/// </summary>
		private event EventHandler<DicomEventArgs> _moveScpBeginEvent;
		private object _moveScpBeginEventLock = new object();
		/// <summary>
		/// Fires when a C-MOVE query has asked for an update
		/// </summary>
		private event EventHandler<DicomEventArgs> _moveScpProgressEvent;
		private object _moveScpProgressEventLock = new object();
		/// <summary>
		/// Fires when a C-STORE operation has sent a file
		/// </summary>
		private event EventHandler<DicomEventArgs> _storeScuProgressEvent;
		private object _storeScuProgressEventLock = new object();

		/// <summary>
        /// Fires when a C-FIND result is received.
		/// </summary>
		private event EventHandler<DicomEventArgs> _queryResultReceivedEvent;
		private object _queryResultReceivedEventLock = new object();
		/// <summary>
		/// Fires when retrieve progress has changed.
		/// </summary>
		private event EventHandler<DicomEventArgs> _retrieveProgressUpdatedEvent;
		private object _retrieveProgressUpdatedEventLock = new object();
		
		private DicomEventManager()
		{
			_findScpCallbackHelper = new FindScpCallbackHelper(this);
			_storeScpCallbackHelper = new StoreScpCallbackHelper(this);
			_moveScpCallbackHelper = new MoveScpCallbackHelper(this);

			_queryCallbackHelper = new QueryCallbackHelper(this);
			_storeScuCallbackHelper = new StoreScuCallbackHelper(this);
			_retrieveCallbackHelper = new RetrieveCallbackHelper(this);
		}

		~DicomEventManager()
		{
			UnregisterCallbacks();
		}

		/// <summary>
		/// The one and only instance of the Dicom Event Manager (per process, anyway).
		/// </summary>
		public static DicomEventManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DicomEventManager();

				return _instance;
			}
		}

		public event EventHandler<DicomEventArgs> FindScpEvent
        {
			add
			{
				lock (_findScpEventLock)
				{
					_findScpEvent += value;
				}
			}
            remove
			{
				lock (_findScpEventLock)
				{
					_findScpEvent -= value;
				}
			}
        }
        public event EventHandler<DicomEventArgs> FindScpProgressEvent
        {
			add
			{
				lock (_findScpProgressEventLock)
				{
					_findScpProgressEvent += value;
				}
			}
			remove
			{
				lock (_findScpProgressEventLock)
				{
					_findScpProgressEvent -= value;
				}
			}
		}
        public event EventHandler<DicomEventArgs> StoreScpBeginEvent
        {
			add
			{
				lock (_storeScpBeginEventLock)
				{
					_storeScpBeginEvent += value;
				}
			}
			remove
			{
				lock (_storeScpBeginEventLock)
				{
					_storeScpBeginEvent -= value;
				}
			}
		}
        public event EventHandler<DicomEventArgs> StoreScpProgressEvent
        {
			add
			{
				lock (_storeScpProgressEventLock)
				{
					_storeScpProgressEvent += value;
				}
			}
			remove
			{
				lock (_storeScpProgressEventLock)
				{
					_storeScpProgressEvent -= value;
				}
			}
		}
        public event EventHandler<DicomEventArgs> StoreScpEndEvent
        {
			add
			{
				lock (_storeScpEndEventLock)
				{
					_storeScpEndEvent += value;
				}
			}
			remove
			{
				lock (_storeScpEndEventLock)
				{
					_storeScpEndEvent -= value;
				}
			}
		}
        public event EventHandler<DicomEventArgs> MoveScpBeginEvent
        {
			add
			{
				lock (_moveScpBeginEventLock)
				{
					_moveScpBeginEvent += value;
				}
			}
			remove
			{
				lock (_moveScpBeginEventLock)
				{
					_moveScpBeginEvent -= value;
				}
			}
		}
        public event EventHandler<DicomEventArgs> MoveScpProgressEvent
        {
			add
			{
				lock (_moveScpProgressEventLock)
				{
					_moveScpProgressEvent += value;
				}
			}
			remove
			{
				lock (_moveScpProgressEventLock)
				{
					_moveScpProgressEvent -= value;
				}
			}
		}

        public event EventHandler<DicomEventArgs> StoreScuProgressEvent
        {
			add
			{
				lock (_storeScuProgressEventLock)
				{
					_storeScuProgressEvent += value;
				}
			}
			remove
			{
				lock (_storeScuProgressEventLock)
				{
					_storeScuProgressEvent -= value;
				}
			}
		}

		public event EventHandler<DicomEventArgs> QueryResultReceivedEvent
		{
			add
			{
				lock (_queryResultReceivedEventLock)
				{
					_queryResultReceivedEvent += value;
				}
			}
			remove
			{
				lock (_queryResultReceivedEventLock)
				{
					_queryResultReceivedEvent -= value;
				}
			}
		}

		public event EventHandler<DicomEventArgs> RetrieveProgressUpdatedEvent
		{
			add
			{
				lock (_retrieveProgressUpdatedEventLock)
				{
					_retrieveProgressUpdatedEvent += value;
				}
			}
			remove
			{
				lock (_retrieveProgressUpdatedEventLock)
				{
					_retrieveProgressUpdatedEvent -= value;
				}
			}
		}

		protected void OnFindScpEvent(DicomEventArgs e)
        {
			lock (_findScpEventLock)
			{
				EventsHelper.Fire(_findScpEvent, this, e);
			}
        }
        
		protected void OnFindScpProgressEvent(DicomEventArgs e)
        {
			lock (_findScpProgressEventLock)
			{
				EventsHelper.Fire(_findScpProgressEvent, this, e);
			}
        }
        
		protected void OnStoreScpBeginEvent(DicomEventArgs e)
        {
			lock (_storeScpBeginEventLock)
			{
				EventsHelper.Fire(_storeScpBeginEvent, this, e);
			}
        }
        
		protected void OnStoreScpProgressEvent(DicomEventArgs e)
        {
			lock (_storeScpProgressEventLock)
			{
				EventsHelper.Fire(_storeScpProgressEvent, this, e);
			}
        }
        
		protected void OnStoreScpEndEvent(DicomEventArgs e)
        {
			lock (_storeScpEndEventLock)
			{
				EventsHelper.Fire(_storeScpEndEvent, this, e);
			}
        }
        
		protected void OnMoveScpBeginEvent(DicomEventArgs e)
        {
			lock (_moveScpBeginEventLock)
			{
				EventsHelper.Fire(_moveScpBeginEvent, this, e);
			}
        }
        
		protected void OnMoveScpProgressEvent(DicomEventArgs e)
        {
			lock (_moveScpProgressEventLock)
			{
				EventsHelper.Fire(_moveScpProgressEvent, this, e);
			}
        }

        protected void OnStoreScuProgressEvent(DicomEventArgs e)
        {
			lock (_storeScuProgressEventLock)
			{
				EventsHelper.Fire(_storeScuProgressEvent, this, e);
			}
        }

		protected void OnQueryResultReceivedEvent(DicomEventArgs e)
		{
			lock (_queryResultReceivedEventLock)
			{
				EventsHelper.Fire(_queryResultReceivedEvent, this, e);
			}
		}

		protected void OnRetrieveProgressUpdatedEvent(DicomEventArgs e)
		{
			lock (_retrieveProgressUpdatedEventLock)
			{
				EventsHelper.Fire(_retrieveProgressUpdatedEvent, this, e);
			}
		}

		private class FindScpCallbackHelper : IDisposable
        {
            private FindScpCallbackHelper_QueryDBDelegate _findScpCallbackHelper_QueryDBDelegate;
            private FindScpCallbackHelper_GetNextResponseDelegate _findScpCallbackHelper_GetNextResponseDelegate;
			private DicomEventManager _parent;

			public FindScpCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;

                _findScpCallbackHelper_QueryDBDelegate = new FindScpCallbackHelper_QueryDBDelegate(QueryDBCallback);
                _findScpCallbackHelper_GetNextResponseDelegate = new FindScpCallbackHelper_GetNextResponseDelegate(GetNextResponseCallback);
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(_findScpCallbackHelper_QueryDBDelegate);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(_findScpCallbackHelper_GetNextResponseDelegate);
            }

            ~FindScpCallbackHelper()
            {
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(null);
            }

            public delegate void FindScpCallbackHelper_QueryDBDelegate(IntPtr interopFindScpCallbackInfo);
            public delegate void FindScpCallbackHelper_GetNextResponseDelegate(IntPtr interopFindScpCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterFindScpCallbackHelper_QueryDB_OffisDcm")]
            public static extern void RegisterFindScpCallbackHelper_QueryDB_OffisDcm(FindScpCallbackHelper_QueryDBDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm")]
            public static extern void RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(FindScpCallbackHelper_GetNextResponseDelegate callbackDelegate);

            private void QueryDBCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                _parent.OnFindScpEvent(new DicomEventArgs(interopFindScpCallbackInfoPointer));
            }

            private void GetNextResponseCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                _parent.OnFindScpProgressEvent(new DicomEventArgs(interopFindScpCallbackInfoPointer));
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(null);
            }

            #endregion
        }

        private class StoreScpCallbackHelper : IDisposable
        {
            private StoreScpCallbackHelper_StoreBeginDelegate _storeScpCallbackHelper_StoreBeginDelegate;
            private StoreScpCallbackHelper_StoreProgressDelegate _storeScpCallbackHelper_StoreProgressDelegate;
            private StoreScpCallbackHelper_StoreEndDelegate _storeScpCallbackHelper_StoreEndDelegate;
			private DicomEventManager _parent;

			public StoreScpCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;

                _storeScpCallbackHelper_StoreBeginDelegate = new StoreScpCallbackHelper_StoreBeginDelegate(StoreBeginCallback);
                _storeScpCallbackHelper_StoreProgressDelegate = new StoreScpCallbackHelper_StoreProgressDelegate(StoreProgressCallback);
                _storeScpCallbackHelper_StoreEndDelegate = new StoreScpCallbackHelper_StoreEndDelegate(StoreEndCallback);

                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(_storeScpCallbackHelper_StoreBeginDelegate);
                RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm(_storeScpCallbackHelper_StoreProgressDelegate);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(_storeScpCallbackHelper_StoreEndDelegate);
            }

            ~StoreScpCallbackHelper()
            {
                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(null);
            }

            public delegate void StoreScpCallbackHelper_StoreBeginDelegate(IntPtr interopStoreCallbackInfo);
            public delegate void StoreScpCallbackHelper_StoreProgressDelegate(IntPtr interopStoreCallbackInfo);
            public delegate void StoreScpCallbackHelper_StoreEndDelegate(IntPtr interopStoreCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(StoreScpCallbackHelper_StoreBeginDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm(StoreScpCallbackHelper_StoreProgressDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(StoreScpCallbackHelper_StoreEndDelegate callbackDelegate);

            private void StoreBeginCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                _parent.OnStoreScpBeginEvent(new DicomEventArgs(interopStoreScpCallbackInfoPointer));
            }

            private void StoreProgressCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                _parent.OnStoreScpProgressEvent(new DicomEventArgs(interopStoreScpCallbackInfoPointer));
            }

            private void StoreEndCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                _parent.OnStoreScpEndEvent(new DicomEventArgs(interopStoreScpCallbackInfoPointer));
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreProgress_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(null);
            }

            #endregion
        }

        private class MoveScpCallbackHelper : IDisposable
        {
            private MoveScpCallbackHelper_MoveBeginDelegate _movedScpCallbackHelper_MoveBeginDelegate;
            private MoveScpCallbackHelper_MoveNextResponseDelegate _moveScpCallbackHelper_MoveNextResponseDelegate;
			private DicomEventManager _parent;

			public MoveScpCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;

                _movedScpCallbackHelper_MoveBeginDelegate = new MoveScpCallbackHelper_MoveBeginDelegate(MoveBeginCallback);
                _moveScpCallbackHelper_MoveNextResponseDelegate = new MoveScpCallbackHelper_MoveNextResponseDelegate(MoveNextResponseCallback);
                RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm(_movedScpCallbackHelper_MoveBeginDelegate);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(_moveScpCallbackHelper_MoveNextResponseDelegate);
            }

            ~MoveScpCallbackHelper()
            {
                RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm(null);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(null);
            }

            public delegate void MoveScpCallbackHelper_MoveBeginDelegate(IntPtr interopMoveScpCallbackInfo);
            public delegate void MoveScpCallbackHelper_MoveNextResponseDelegate(IntPtr interopMoveScpCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm")]
            public static extern void RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm(MoveScpCallbackHelper_MoveBeginDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm")]
            public static extern void RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(MoveScpCallbackHelper_MoveNextResponseDelegate callbackDelegate);

            private void MoveBeginCallback(IntPtr interopMoveScpCallbackInfoPointer)
            {
                _parent.OnMoveScpBeginEvent(new DicomEventArgs(interopMoveScpCallbackInfoPointer));
            }

            private void MoveNextResponseCallback(IntPtr interopMoveScpCallbackInfoPointer)
            {
                _parent.OnMoveScpProgressEvent(new DicomEventArgs(interopMoveScpCallbackInfoPointer));
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterMoveScpCallbackHelper_MoveBegin_OffisDcm(null);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(null);
            }

            #endregion
        }

        private class StoreScuCallbackHelper : IDisposable
        {
			public StoreScuCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;
                _storeScuCallbackHelperDelegate = new StoreScuCallbackHelperDelegate(DefaultCallback);
                RegisterStoreScuCallbackHelper_OffisDcm(_storeScuCallbackHelperDelegate);
            }

			~StoreScuCallbackHelper()
			{
				RegisterStoreScuCallbackHelper_OffisDcm(null);
			}

            public delegate void StoreScuCallbackHelperDelegate(IntPtr interopStoreScuCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScuCallbackHelper_OffisDcm")]
            public static extern void RegisterStoreScuCallbackHelper_OffisDcm(StoreScuCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreScuCallbackInfo)
            {
                _parent.OnStoreScuProgressEvent(new DicomEventArgs(interopStoreScuCallbackInfo));
            }

            private StoreScuCallbackHelperDelegate _storeScuCallbackHelperDelegate;
			private DicomEventManager _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScuCallbackHelper_OffisDcm(null);
            }

			#endregion
		}

		private class QueryCallbackHelper : IDisposable
        {
			public QueryCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;
                _queryCallbackHelperDelegate = new QueryCallbackHelperDelegate(DefaultCallback);
                RegisterQueryCallbackHelper_OffisDcm(_queryCallbackHelperDelegate);
            }

            public delegate void QueryCallbackHelperDelegate(IntPtr callbackData);

            [DllImport("OffisDcm", EntryPoint = "RegisterQueryCallbackHelper_OffisDcm")]
            public static extern void RegisterQueryCallbackHelper_OffisDcm(QueryCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr callbackData)
            {
				_parent.OnQueryResultReceivedEvent(new DicomEventArgs(callbackData));
			}

            private QueryCallbackHelperDelegate _queryCallbackHelperDelegate;
			private DicomEventManager _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterQueryCallbackHelper_OffisDcm(null);
            }

            #endregion
        }

        private class RetrieveCallbackHelper : IDisposable
        {
            public RetrieveCallbackHelper(DicomEventManager parent)
            {
                _parent = parent;
                _retrieveCallbackHelperDelegate = new RetrieveCallbackHelperDelegate(DefaultCallback);
                RegisterRetrieveCallbackHelper_OffisDcm(_retrieveCallbackHelperDelegate);
            }

            public delegate void RetrieveCallbackHelperDelegate(IntPtr callbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterRetrieveCallbackHelper_OffisDcm")]
            public static extern void RegisterRetrieveCallbackHelper_OffisDcm(RetrieveCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr callbackInfoPointer)
            {
				_parent.OnRetrieveProgressUpdatedEvent(new DicomEventArgs(callbackInfoPointer));
            }

            private RetrieveCallbackHelperDelegate _retrieveCallbackHelperDelegate;
			private DicomEventManager _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterRetrieveCallbackHelper_OffisDcm(null);
            }

            #endregion
        }

		private void UnregisterCallbacks()
		{
			if (_findScpCallbackHelper != null)
			{
				_findScpCallbackHelper.Dispose();
				_findScpCallbackHelper = null;
			}

			if (_storeScpCallbackHelper != null)
			{
				_storeScpCallbackHelper.Dispose();
				_storeScpCallbackHelper = null;
			}

			if (_moveScpCallbackHelper != null)
			{
				_moveScpCallbackHelper.Dispose();
				_moveScpCallbackHelper = null;
			}

			if (_storeScuCallbackHelper != null)
			{
				_storeScuCallbackHelper.Dispose();
				_storeScuCallbackHelper = null;
			}

			if (_queryCallbackHelper != null)
			{
				_queryCallbackHelper.Dispose();
				_queryCallbackHelper = null;
			}

			if (_retrieveCallbackHelper != null)
			{
				_retrieveCallbackHelper.Dispose();
				_retrieveCallbackHelper = null;
			}
		}

		#region IDisposable Members

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnregisterCallbacks();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}


