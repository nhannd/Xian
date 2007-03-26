using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.Network
{
    public partial class DicomServer
    {
        /// <summary>
        /// Fires when a C-FIND query has been received
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _findScpEvent;
        /// <summary>
        /// Fires when a C-FIND query has asked for an update
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _findScpProgressEvent;
        /// <summary>
        /// Fires when a new SOP Instance has arrived and is starting to be written to the local filesystem.
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _storeScpBeginEvent;
        /// <summary>
        /// Fires when more data of the new SOP Instance has arrived
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _storeScpProgressEvent;
        /// <summary>
        /// Fires when a new SOP Instance has been successfully written to the local filesystem.
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _storeScpEndEvent;
        /// <summary>
        /// Fires when a C-MOVE query has been received
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _moveScpBeginEvent;
        /// <summary>
        /// Fires when a C-MOVE query has asked for an update
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _moveScpProgressEvent;
        /// <summary>
        /// Fires when a C-STORE operation has sent a file
        /// </summary>
        private event EventHandler<DicomServerEventArgs> _storeScuProgressEvent;

		public event EventHandler<DicomServerEventArgs> FindScpEvent
        {
            add { _findScpEvent += value; }
            remove { _findScpEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> FindScpProgressEvent
        {
            add { _findScpProgressEvent += value; }
            remove { _findScpProgressEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> StoreScpBeginEvent
        {
            add { _storeScpBeginEvent += value; }
            remove { _storeScpBeginEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> StoreScpProgressEvent
        {
            add { _storeScpProgressEvent += value; }
            remove { _storeScpProgressEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> StoreScpEndEvent
        {
            add { _storeScpEndEvent += value; }
            remove { _storeScpEndEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> MoveScpBeginEvent
        {
            add { _moveScpBeginEvent += value; }
            remove { _moveScpBeginEvent -= value; }
        }
        public event EventHandler<DicomServerEventArgs> MoveScpProgressEvent
        {
            add { _moveScpProgressEvent += value; }
            remove { _moveScpProgressEvent -= value; }
        }

        public event EventHandler<DicomServerEventArgs> StoreScuProgressEvent
        {
            add { _storeScuProgressEvent += value; }
            remove { _storeScuProgressEvent -= value; }
        }

        protected void OnFindScpEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_findScpEvent, this, e);
        }
        protected void OnFindScpProgressEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_findScpProgressEvent, this, e);
        }
        protected void OnStoreScpBeginEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_storeScpBeginEvent, this, e);
        }
        protected void OnStoreScpProgressEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_storeScpProgressEvent, this, e);
        }
        protected void OnStoreScpEndEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_storeScpEndEvent, this, e);
        }
        protected void OnMoveScpBeginEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_moveScpBeginEvent, this, e);
        }
        protected void OnMoveScpProgressEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_moveScpProgressEvent, this, e);
        }

        protected void OnStoreScuProgressEvent(DicomServerEventArgs e)
        {
            EventsHelper.Fire(_storeScuProgressEvent, this, e);
        }

		private class FindScpCallbackHelper : IDisposable
        {
            private FindScpCallbackHelper_QueryDBDelegate _findScpCallbackHelper_QueryDBDelegate;
            private FindScpCallbackHelper_GetNextResponseDelegate _findScpCallbackHelper_GetNextResponseDelegate;
            private DicomServer _parent;

            public FindScpCallbackHelper(DicomServer parent)
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
                _parent.OnFindScpEvent(new DicomServerEventArgs(interopFindScpCallbackInfoPointer));
            }

            private void GetNextResponseCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                _parent.OnFindScpProgressEvent(new DicomServerEventArgs(interopFindScpCallbackInfoPointer));
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
            private DicomServer _parent;

            public StoreScpCallbackHelper(DicomServer parent)
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
                _parent.OnStoreScpBeginEvent(new DicomServerEventArgs(interopStoreScpCallbackInfoPointer));
            }

            private void StoreProgressCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                _parent.OnStoreScpProgressEvent(new DicomServerEventArgs(interopStoreScpCallbackInfoPointer));
            }

            private void StoreEndCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                _parent.OnStoreScpEndEvent(new DicomServerEventArgs(interopStoreScpCallbackInfoPointer));
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
            private DicomServer _parent;

            public MoveScpCallbackHelper(DicomServer parent)
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
                _parent.OnMoveScpBeginEvent(new DicomServerEventArgs(interopMoveScpCallbackInfoPointer));
            }

            private void MoveNextResponseCallback(IntPtr interopMoveScpCallbackInfoPointer)
            {
                _parent.OnMoveScpProgressEvent(new DicomServerEventArgs(interopMoveScpCallbackInfoPointer));
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
            public StoreScuCallbackHelper(DicomServer parent)
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
                _parent.OnStoreScuProgressEvent(new DicomServerEventArgs(interopStoreScuCallbackInfo));
            }

            private StoreScuCallbackHelperDelegate _storeScuCallbackHelperDelegate;
            private DicomServer _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScuCallbackHelper_OffisDcm(null);
            }

			#endregion
		}
    }
}


