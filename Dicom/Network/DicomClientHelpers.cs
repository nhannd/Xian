using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;

namespace ClearCanvas.Dicom.Network
{
    public partial class DicomClient
    {
        class QueryCallbackHelper : IDisposable
        {
            public QueryCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _queryCallbackHelperDelegate = new QueryCallbackHelperDelegate(DefaultCallback);
                RegisterQueryCallbackHelper_OffisDcm(_queryCallbackHelperDelegate);
            }

            public delegate void QueryCallbackHelperDelegate(IntPtr callbackData,
                                                             IntPtr request,
                                                             int responseCount,
                                                             IntPtr response,
                                                             IntPtr responseIdentifiers);

            [DllImport("OffisDcm", EntryPoint = "RegisterQueryCallbackHelper_OffisDcm")]
            public static extern void RegisterQueryCallbackHelper_OffisDcm(QueryCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr callbackData,
                                        IntPtr request,
                                        int responseCount,
                                        IntPtr response,
                                        IntPtr responseIdentifiers)
            {
                T_DIMSE_C_FindRQ outboundRequest = new T_DIMSE_C_FindRQ(request, false);
                T_DIMSE_C_FindRSP inboundResponse = new T_DIMSE_C_FindRSP(response, false);
                DcmDataset responseData = new DcmDataset(responseIdentifiers, false);

                if (DICOM_PENDING_STATUS(inboundResponse.DimseStatus) ||
                    DICOM_SUCCESS_STATUS(inboundResponse.DimseStatus))
                {
                    QueryResult queryResult = null;
                    DcmObject item = responseData.nextInContainer(null);
                    bool tagAvailable = (item != null);

                    // there actually is a result
                    if (tagAvailable)
                    {
                        queryResult = new QueryResult();

                        while (tagAvailable)
                        {
                            DcmElement element = OffisDcm.castToDcmElement(item);
                            if (null != element)
                            {
                                queryResult.Add(new DicomTag(element.getGTag(), element.getETag()), element.ToString());
                            }

                            item = responseData.nextInContainer(item);
                            tagAvailable = (item != null);
                        }

                        _parent._queryResults.Add(queryResult);

                        QueryResultReceivedEventArgs args = new QueryResultReceivedEventArgs(queryResult);
                        _parent.OnQueryResultReceivedEvent(args);
                    }
                }
            }

            private QueryCallbackHelperDelegate _queryCallbackHelperDelegate;
            private DicomClient _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterQueryCallbackHelper_OffisDcm(null);
            }

            #endregion
        }

        class RetrieveCallbackHelper : IDisposable
        {
            public RetrieveCallbackHelper(DicomClient parent)
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
                InteropRetrieveCallbackInfo callbackInfo = new InteropRetrieveCallbackInfo(callbackInfoPointer, false);
                T_DIMSE_C_MoveRSP moveResponse = callbackInfo.CMoveResponse;

                if ((moveResponse.opts & OffisDcm.O_MOVE_NUMBEROFCOMPLETEDSUBOPERATIONS) > 0 &&
                    (moveResponse.opts & OffisDcm.O_MOVE_NUMBEROFFAILEDSUBOPERATIONS) > 0 &&
                    (moveResponse.opts & OffisDcm.O_MOVE_NUMBEROFREMAININGSUBOPERATIONS) > 0)
                {
                    int completedOperations = moveResponse.NumberOfCompletedSubOperations;
                    int failedOperations = moveResponse.NumberOfFailedSubOperations;
                    int remainingOperations = moveResponse.NumberOfRemainingSubOperations;

                    RetrieveProgressUpdatedEventArgs args = new RetrieveProgressUpdatedEventArgs(completedOperations, failedOperations, remainingOperations);
                    _parent.OnRetrieveProgressUpdatedEvent(args);
                }
            }

            private RetrieveCallbackHelperDelegate _retrieveCallbackHelperDelegate;
            private DicomClient _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterRetrieveCallbackHelper_OffisDcm(null);
            }

            #endregion
        }

        /*
        class StoreCallbackHelper : IDisposable
        {
            public StoreCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _storeCallbackHelperDelegate = new StoreCallbackHelperDelegate(DefaultCallback);
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(_storeCallbackHelperDelegate);
            }

            public delegate void StoreCallbackHelperDelegate(IntPtr interopStoreCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm")]
            public static extern void RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(StoreCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreCallbackInfo)
            {
                InteropStoreScpCallbackInfo callbackInfo = new InteropStoreScpCallbackInfo(interopStoreCallbackInfo, false);
                string fileName = callbackInfo.FileName;
                DcmDataset imageDataset = callbackInfo.ImageDataset;

                // make a copy of the string, since the string passed in is 
                // allocated on the stack and will be gone very soon
                StringBuilder fileNameString = new StringBuilder();
                fileNameString.Append(fileName);

                SopInstanceReceivedEventArgs args = new SopInstanceReceivedEventArgs(fileNameString.ToString());
                _parent.OnSopInstanceReceivedEvent(args);
            }

            private StoreCallbackHelperDelegate _storeCallbackHelperDelegate;
            private DicomClient _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScpCallbackHelper_StoreEnd_OffisDcm(null);
            }

            #endregion
        }
        */

        class StoreScuCallbackHelper : IDisposable
        {
            public StoreScuCallbackHelper(DicomClient parent)
            {
                _parent = parent;
                _storeScuCallbackHelperDelegate = new StoreScuCallbackHelperDelegate(DefaultCallback);
                RegisterStoreScuCallbackHelper_OffisDcm(_storeScuCallbackHelperDelegate);
            }

            public delegate void StoreScuCallbackHelperDelegate(IntPtr interopStoreScuCallbackInfo);

            [DllImport("OffisDcm", EntryPoint = "RegisterStoreScuCallbackHelper_OffisDcm")]
            public static extern void RegisterStoreScuCallbackHelper_OffisDcm(StoreScuCallbackHelperDelegate callbackDelegate);

            public void DefaultCallback(IntPtr interopStoreScuCallbackInfo)
            {
                InteropStoreScuCallbackInfo callbackInfo = new InteropStoreScuCallbackInfo(interopStoreScuCallbackInfo, false);
                T_DIMSE_C_StoreRQ request = callbackInfo.Request;
                T_DIMSE_StoreProgress progress = callbackInfo.Progress;

                _parent.OnSendProgressUpdatedEvent(new SendProgressUpdatedEventArgs(callbackInfo.CurrentCount, callbackInfo.TotalCount));
            }

            private StoreScuCallbackHelperDelegate _storeScuCallbackHelperDelegate;
            private DicomClient _parent;

            #region IDisposable Members

            public void Dispose()
            {
                RegisterStoreScuCallbackHelper_OffisDcm(null);
            }

            #endregion
        }
    }
}
