using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.Network
{
    public partial class DicomServer
    {
        class FindScpCallbackHelper : IDisposable
        {
            private FindScpCallbackHelper_QueryDBDelegate _findScpCallbackHelper_QueryDBDelegate;
            private FindScpCallbackHelper_GetNextResponseDelegate _findScpCallbackHelper_GetNextResponseDelegate;
            private DicomServer _parent;
            private ReadOnlyQueryResultCollection _queryResults;
            private int _resultIndex;

            public FindScpCallbackHelper(DicomServer parent)
            {
                _parent = parent;
                _queryResults = null;
                _resultIndex = 0;

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
                InteropFindScpCallbackInfo interopFindScpCallbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);

                T_DIMSE_C_FindRQ request = interopFindScpCallbackInfo.Request;
                T_DIMSE_C_FindRSP response = interopFindScpCallbackInfo.Response;
                DcmDataset requestIdentifiers = interopFindScpCallbackInfo.RequestIdentifiers;

                try
                {
                    _resultIndex = 0;
                    _queryResults = null;

                    QueryKey queryKey = BuildQueryKey(requestIdentifiers);
                    FindScpEventArgs args = new FindScpEventArgs(queryKey);
                    _parent.OnFindScpEvent(args);
                    _queryResults = args.QueryResults;

                    response.MessageIDBeingRespondedTo = request.MessageID;
                    response.AffectedSOPClassUID = request.AffectedSOPClassUID;
                    if (_queryResults.Count == 0)
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                    else
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Pending;
                }
                catch
                {
                    response.DimseStatus = (ushort)OffisDcm.STATUS_FIND_Failed_UnableToProcess;
                }
            }

            private void GetNextResponseCallback(IntPtr interopFindScpCallbackInfoPointer)
            {
                InteropFindScpCallbackInfo interopFindScpCallbackInfo = new InteropFindScpCallbackInfo(interopFindScpCallbackInfoPointer, false);

                T_DIMSE_C_FindRQ request = interopFindScpCallbackInfo.Request;
                T_DIMSE_C_FindRSP response = interopFindScpCallbackInfo.Response;
                DcmDataset requestIdentifiers = interopFindScpCallbackInfo.RequestIdentifiers;
                DcmDataset responseIdentifiers = interopFindScpCallbackInfo.ResponseIdentifiers;

                try
                {
                    response.MessageIDBeingRespondedTo = request.MessageID;
                    response.AffectedSOPClassUID = request.AffectedSOPClassUID;

                    if (_queryResults.Count == 0 || _resultIndex >= _queryResults.Count)
                        response.DimseStatus = (ushort)OffisDcm.STATUS_Success;
                    else
                    {
                        GetQueryResult(_resultIndex, responseIdentifiers);
                        _resultIndex++;
                    }
                }
                catch
                {
                    response.DimseStatus = (ushort)OffisDcm.STATUS_FIND_Failed_UnableToProcess;
                }
            }

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

                return queryKey;
            }

            private void GetQueryResult(int index, DcmDataset responseIdentifiers)
            {
                QueryResult result = _queryResults[index];

                // TODO:  edit these when we need to expand the list of supported return tags
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
    }
}


