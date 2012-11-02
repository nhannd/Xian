#region License

//Copyright (C)  2012 Aaron Boxer

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using DataCache;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    public class PixelStreamer
    {
        private static readonly ResultSynchronizer Synchronizer;
        private readonly Uri _baseUri;

        static PixelStreamer()
        {
            Synchronizer = new ResultSynchronizer();
        }

        public PixelStreamer(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public void FetchPixels(FetchInfo fetchInfo)
        {
            try
            {
                var result = new FrameStreamingResultMetaData();
                var url = new StringBuilder();

                url.AppendFormat(_baseUri.ToString().EndsWith("/") ? "{0}{1}" : "{0}/{1}", _baseUri, fetchInfo.ServerAE);
                url.AppendFormat("?requesttype=WADO&studyUID={0}&seriesUID={1}&objectUID={2}", fetchInfo.StudyInstanceUID,
                                 fetchInfo.SeriesInstanceUID, fetchInfo.SopInstanceUid);
                url.AppendFormat("&frameNumber={0}", fetchInfo.FrameNumber);
                url.AppendFormat("&contentType={0}", HttpUtility.HtmlEncode("application/clearcanvas"));

                result.Speed.Start();

                var request = (HttpWebRequest) WebRequest.Create(url.ToString());
                request.Accept = "application/dicom,application/clearcanvas,image/jpeg";
                request.Timeout =
                    (int) TimeSpan.FromSeconds(StreamingSettings.Default.ClientTimeoutSeconds).TotalMilliseconds;
                request.KeepAlive = false;
                var requestInfo = new RequestInfo
                                       {
                                           Request = request,
                                           Result = result,
                                           EndGetCallback = fetchInfo.EndGetCallback,
                                           PostResponseCallback = fetchInfo.PostResponseCallback,
                                           FrameNumber = fetchInfo.FrameNumber,
                                           SopInstanceUid = fetchInfo.SopInstanceUid,
                                           CacheId = fetchInfo.CacheId,
                                           Asynch = fetchInfo.Asynch
                                       };
                // asynch call
                if (fetchInfo.Asynch)
                {
                    var acquireResult = Synchronizer.Acquire(fetchInfo.CacheId, ResultSynchronizer.AcquireType.CreateOnly);
                    if (!acquireResult.Created)
                        return;

                    request.BeginGetResponse(ResponseCallback, requestInfo);
                    if (fetchInfo.BeginGetCallback != null)
                        fetchInfo.BeginGetCallback();
                    return;
                }

                //synch call
                using (var waiter = Synchronizer.GetResultWaiter(fetchInfo.CacheId))
                {
                    if (!waiter.Wait(10000))
                    {
                        HttpWebResponse response;
                        try
                        {
                            if (fetchInfo.BeginGetCallback != null)
                                fetchInfo.BeginGetCallback();
                            response = request.GetResponse() as HttpWebResponse;
                        }
                        finally
                        {
                            if (fetchInfo.EndGetCallback != null)
                                fetchInfo.EndGetCallback();
                        }
                        if (response != null)
                            ReadResponse(requestInfo, response);
                    }
                       
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response is HttpWebResponse)
                {
                    var response = (HttpWebResponse) ex.Response;
                    throw new StreamingClientException(response.StatusCode,
                                                       HttpUtility.HtmlDecode(response.StatusDescription));
                }
                throw new StreamingClientException(StreamingClientExceptionType.Network, ex);
            }
        }

        private static void ReadResponse(RequestInfo requestInfo, HttpWebResponse response)
        {
            try
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new StreamingClientException(response.StatusCode,
                                                       HttpUtility.HtmlDecode(response.StatusDescription));
                }

                var compressed = (response.Headers["Compressed"] != null && bool.Parse(response.Headers["Compressed"]));
                var responseLength = (int) response.ContentLength;
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                        throw new Exception("");
                    if (requestInfo.PostResponseCallback != null)
                        requestInfo.PostResponseCallback(responseStream, responseLength, compressed);
                    Synchronizer.Set(requestInfo.CacheId);
                }

                var result = requestInfo.Result;

                result.Speed.SetData(responseLength);
                result.Speed.End();

                result.ResponseMimeType = response.ContentType;
                result.Status = response.StatusCode;
                result.StatusDescription = response.StatusDescription;
                result.Uri = response.ResponseUri;
                result.ContentLength = responseLength;
                result.IsLast = (response.Headers["IsLast"] != null && bool.Parse(response.Headers["IsLast"]));

                PerformanceReportBroker.PublishReport("Streaming", "RetrievePixelData",
                                                      result.Speed.ElapsedTime.TotalSeconds);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, "[PixelStreamer]: Read Exception : {0}", e);
                throw;
            }
            finally
            {
                if (requestInfo.Asynch)
                    Synchronizer.Release(requestInfo.CacheId);
            }
        }

        private static void ResponseCallback(IAsyncResult asyncResult)
        {
            var requestState = (RequestInfo) asyncResult.AsyncState;
            var request = requestState.Request;
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse) request.EndGetResponse(asyncResult);
                ReadResponse(requestState, response);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, "[PixelStreamer] Exception  in response callback: {0}", e);
                if (response == null)
                {
                    Synchronizer.Release(requestState.CacheId);
                }
            }
            finally
            {
                if (requestState.EndGetCallback != null)
                    requestState.EndGetCallback();
            }
        }

        #region Nested type: RequestBase

        public class RequestBase
        {
            public string CacheId;
            public int FrameNumber;
            public string SopInstanceUid;
        }

        #endregion

        #region Nested type: RequestState

        private class RequestInfo : RequestBase
        {
            public bool Asynch;
            public Action EndGetCallback;
            public HttpWebRequest Request;
            public FrameStreamingResultMetaData Result;
            public Action<Stream, int, bool> PostResponseCallback;
        }

        #endregion

        #region Nested type: RetrieveArgs

        public class FetchInfo : RequestBase
        {
            public bool Asynch;
            public Action BeginGetCallback;
            public Action EndGetCallback;
            public Action<Stream, int, bool> PostResponseCallback;
            public string SeriesInstanceUID;
            public string ServerAE;
            public string StudyInstanceUID;
            public string TransferSyntaxUID;
        }

        #endregion
    }
}