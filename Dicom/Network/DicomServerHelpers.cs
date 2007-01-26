using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.Network
{
    public partial class DicomServer
    {
        // Used by CFind and CMove
        private ReadOnlyQueryResultCollection _queryResults;
        private int _resultIndex;

        private QueryKey BuildQueryKey(DcmDataset requestIdentifiers)
        {
            OFCondition cond;
            QueryKey queryKey = new QueryKey();

            // TODO: shouldn't hard code the buffer length like this
            StringBuilder buf = new StringBuilder(1024);

            // TODO: Edit these when we need to expand the support of search parameters
            cond = requestIdentifiers.findAndGetOFString(Dcm.PatientId, buf);
            if (cond.good())
                queryKey.Add(DicomTag.PatientId, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.AccessionNumber, buf);
            if (cond.good())
                queryKey.Add(DicomTag.AccessionNumber, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.PatientsName, buf);
            if (cond.good())
                queryKey.Add(DicomTag.PatientsName, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyDate, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyDate, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyDescription, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyDescription, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.ModalitiesInStudy, buf);
            if (cond.good())
                queryKey.Add(DicomTag.ModalitiesInStudy, buf.ToString());

            cond = requestIdentifiers.findAndGetOFString(Dcm.StudyInstanceUID, buf);
            if (cond.good())
                queryKey.Add(DicomTag.StudyInstanceUID, buf.ToString());

            return queryKey;
        }

        public int StartQuery(DcmDataset requestIdentifiers)
        {
            try
            {
                _resultIndex = 0;

                FindScpEventArgs arg = new FindScpEventArgs(BuildQueryKey(requestIdentifiers));
                OnFindScpEvent(arg);

                _queryResults = arg.QueryResults;
            }
            catch
            {
                _queryResults = null;
                return OffisDcm.STATUS_FIND_Failed_UnableToProcess;
            }

            // query success means query has completed
            if (_queryResults.Count == 0)
                return OffisDcm.STATUS_Success;

            return OffisDcm.STATUS_Pending;
        }

        public int GetNextQueryResult(DcmDataset responseIdentifiers)
        {
            if (_queryResults != null && _queryResults.Count == 0 || _resultIndex >= _queryResults.Count)
            {
                // End of the query results
                _resultIndex = 0;
                _queryResults = null;
                return OffisDcm.STATUS_Success;
            }

            QueryResult result = _queryResults[_resultIndex];

            // TODO:  edit these when we need to expand the list of supported return tags
            responseIdentifiers.clear();
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientId), result.PatientId);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.PatientsName), result.PatientsName);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDate), result.StudyDate);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyTime), result.StudyTime);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyDescription), result.StudyDescription);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.ModalitiesInStudy), result.ModalitiesInStudy);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.AccessionNumber), result.AccessionNumber);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.QueryRetrieveLevel), "STUDY");
            responseIdentifiers.putAndInsertString(new DcmTag(Dcm.StudyInstanceUID), result.StudyInstanceUid);

            _resultIndex++;

            return OffisDcm.STATUS_Pending;       
        }

        public void StartMove(InteropMoveScpCallbackInfo callbackInfo)
        {
            // Start the Query
            callbackInfo.Response.DimseStatus = (ushort)StartQuery(callbackInfo.RequestIdentifiers);
            if (callbackInfo.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                return;

            // Verify we have return results
            callbackInfo.Response.DimseStatus = (ushort)GetNextQueryResult(callbackInfo.ResponseIdentifiers);
            if (callbackInfo.Response.DimseStatus != (ushort)OffisDcm.STATUS_Pending)
                return;

            // Result found and STATUS_Pending, construct MoveScpEventArgs for starting Sub-CStore
            OFCondition cond;
            StringBuilder buf = new StringBuilder(1024);
            String studyInstanceUID = null;
            String studyDescription = null;

            cond = callbackInfo.ResponseIdentifiers.findAndGetOFString(Dcm.StudyInstanceUID, buf);
            if (cond.good())
                studyInstanceUID = buf.ToString();

            if (studyInstanceUID == null || studyInstanceUID.Length == 0)
            {
                callbackInfo.Response.DimseStatus = (ushort)OffisDcm.STATUS_MOVE_Failed_UnableToProcess;
                return;
            }

            cond = callbackInfo.ResponseIdentifiers.findAndGetOFString(Dcm.StudyDescription, buf);
            if (cond.good())
                studyDescription = buf.ToString();

            // Start the Sub-CStore operation
            OnMoveScpBeginEvent(new MoveScpEventArgs(callbackInfo.Request.MessageID, callbackInfo.Request.MoveDestination, studyInstanceUID, studyDescription, callbackInfo.Response));
        }

        class FindScpCallbackHelper : IDisposable
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
                InteropFindScpCallbackInfo callbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);
                callbackInfo.DimseStatus = (ushort) _parent.StartQuery(callbackInfo.RequestIdentifiers);
            }

            private void GetNextResponseCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                InteropFindScpCallbackInfo callbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);
                callbackInfo.DimseStatus = (ushort) _parent.GetNextQueryResult(callbackInfo.ResponseIdentifiers);
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterFindScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterFindScpCallbackHelper_GetNextFindResponse_OffisDcm(null);
            }

            #endregion
        }

        class StoreScpCallbackHelper : IDisposable
        {
            private StoreScpCallbackHelper_StoreBeginDelegate _storeScpCallbackHelper_StoreBeginDelegate;
            private StoreScpCallbackHelper_StoreProgressingDelegate _storeScpCallbackHelper_StoreProgressingDelegate;
            private StoreScpCallbackHelper_StoreEndDelegate _storeScpCallbackHelper_StoreEndDelegate;
            private DicomServer _parent;

            public StoreScpCallbackHelper(DicomServer parent)
            {
                _parent = parent;

                _storeScpCallbackHelper_StoreBeginDelegate = new StoreScpCallbackHelper_StoreBeginDelegate(StoreBeginCallback);
                _storeScpCallbackHelper_StoreProgressingDelegate = new StoreScpCallbackHelper_StoreProgressingDelegate(StoreProgressingCallback);
                _storeScpCallbackHelper_StoreEndDelegate = new StoreScpCallbackHelper_StoreEndDelegate(StoreEndCallback);

                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(_storeScpCallbackHelper_StoreBeginDelegate);
                RegisterStoreScpCallbackHelper_StoreProgressing_OffisDcm(_storeScpCallbackHelper_StoreProgressingDelegate);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(_storeScpCallbackHelper_StoreEndDelegate);
            }

            ~StoreScpCallbackHelper()
            {
                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreProgressing_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(null);
            }

            public delegate void StoreScpCallbackHelper_StoreBeginDelegate(IntPtr interopStoreCallbackInfo);
            public delegate void StoreScpCallbackHelper_StoreProgressingDelegate(IntPtr interopStoreCallbackInfo);
            public delegate void StoreScpCallbackHelper_StoreEndDelegate(IntPtr interopStoreCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(StoreScpCallbackHelper_StoreBeginDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreProgressing_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreProgressing_OffisDcm(StoreScpCallbackHelper_StoreProgressingDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(StoreScpCallbackHelper_StoreEndDelegate callbackDelegate);

            private void StoreBeginCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                InteropStoreScpCallbackInfo callbackInfo = new InteropStoreScpCallbackInfo(interopStoreScpCallbackInfoPointer, false);
                _parent.OnStoreScpBeginEvent(new StoreScpProgressUpdateEventArgs(callbackInfo));
            }

            private void StoreProgressingCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                InteropStoreScpCallbackInfo callbackInfo = new InteropStoreScpCallbackInfo(interopStoreScpCallbackInfoPointer, false);
                _parent.OnStoreScpProgressingEvent(new StoreScpProgressUpdateEventArgs(callbackInfo));
            }

            private void StoreEndCallback(IntPtr interopStoreScpCallbackInfoPointer)
            {
                InteropStoreScpCallbackInfo callbackInfo = new InteropStoreScpCallbackInfo(interopStoreScpCallbackInfoPointer, false);
                _parent.OnStoreScpEndEvent(new StoreScpImageReceivedEventArgs(callbackInfo));
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScpCallbackHelper_StoreBegin_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreProgressing_OffisDcm(null);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(null);
            }

            #endregion
        }

        class MoveScpCallbackHelper : IDisposable
        {
            private MoveScpCallbackHelper_QueryDBDelegate _movedScpCallbackHelper_QueryDBDelegate;
            private MoveScpCallbackHelper_MoveNextResponseDelegate _moveScpCallbackHelper_MoveNextResponseDelegate;
            private DicomServer _parent;

            public MoveScpCallbackHelper(DicomServer parent)
            {
                _parent = parent;

                _movedScpCallbackHelper_QueryDBDelegate = new MoveScpCallbackHelper_QueryDBDelegate(QueryDBCallback);
                _moveScpCallbackHelper_MoveNextResponseDelegate = new MoveScpCallbackHelper_MoveNextResponseDelegate(MoveNextResponseCallback);
                RegisterMoveScpCallbackHelper_QueryDB_OffisDcm(_movedScpCallbackHelper_QueryDBDelegate);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(_moveScpCallbackHelper_MoveNextResponseDelegate);
            }

            ~MoveScpCallbackHelper()
            {
                RegisterMoveScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(null);
            }

            public delegate void MoveScpCallbackHelper_QueryDBDelegate(IntPtr interopMoveScpCallbackInfo);
            public delegate void MoveScpCallbackHelper_MoveNextResponseDelegate(IntPtr interopMoveScpCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterMoveScpCallbackHelper_QueryDB_OffisDcm")]
            public static extern void RegisterMoveScpCallbackHelper_QueryDB_OffisDcm(MoveScpCallbackHelper_QueryDBDelegate callbackDelegate);

            [DllImport("OffisDcm", EntryPoint = "RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm")]
            public static extern void RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(MoveScpCallbackHelper_MoveNextResponseDelegate callbackDelegate);

            private void QueryDBCallback(IntPtr interopMoveScpCallbackInfoPointer)
            {
                InteropMoveScpCallbackInfo callbackInfo = new InteropMoveScpCallbackInfo(interopMoveScpCallbackInfoPointer, false);
                _parent.StartMove(callbackInfo);
            }

            private void MoveNextResponseCallback(IntPtr interopMoveScpCallbackInfoPointer)
            {
                InteropMoveScpCallbackInfo callbackInfo = new InteropMoveScpCallbackInfo(interopMoveScpCallbackInfoPointer, false);
                _parent.OnMoveScpProgressEvent(new MoveScpProgressEventArgs(callbackInfo.Request.MessageID, callbackInfo.Response));
            }

            #region IDisposable Members

            public void Dispose()
            {
                RegisterMoveScpCallbackHelper_QueryDB_OffisDcm(null);
                RegisterMoveScpCallbackHelper_MoveNextResponse_OffisDcm(null);
            }

            #endregion
        }
    }
}


