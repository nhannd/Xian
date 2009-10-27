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

using System.IO;
using System.Net;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.ImageServer.Services.Streaming.Shreds;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    
    /// <summary>
    /// Represents a Dicom WADO request processor.
    /// </summary>
    public class WADORequestProcessor
    {

        #region Private Members

        #endregion
		

        #region Public Properties

        public string ServerAE { get; set; }

        #endregion

        #region Constructors

        #endregion


        #region Private Methods

        /// <summary>
        /// Gets a string that represents the mime-types acceptable by the client for the specified context. The mime-types are separated by commas (,).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetClientAcceptTypes(HttpListenerContext context)
        {
            Platform.CheckForNullReference(context, "context");

            if (context.Request.AcceptTypes == null)
                return null;

            StringBuilder mimes = new StringBuilder();
            foreach (string mime in context.Request.AcceptTypes)
            {
                if (mimes.Length > 0)
                    mimes.Append(",");
                mimes.Append(mime);
            }
            return mimes.ToString();
        }

        /// <summary>
        /// Logs information about the request.
        /// </summary>
        /// <param name="context"></param>
        private static void LogRequest(HttpListenerContext context)
        {
            Platform.CheckForNullReference(context, "context");

            if (Platform.IsLogLevelEnabled(LogLevel.Debug))
            {
                StringBuilder info = new StringBuilder();

                info.AppendFormat("\n\tAgents={0}", context.Request.UserAgent);
                info.AppendFormat("\n\tRequestType={0}", context.Request.QueryString["RequestType"]);
                info.AppendFormat("\n\tStudyUid={0}", context.Request.QueryString["StudyUid"]);
                info.AppendFormat("\n\tSeriesUid={0}", context.Request.QueryString["SeriesUid"]);
                info.AppendFormat("\n\tObjectUid={0}", context.Request.QueryString["ObjectUid"]);
                info.AppendFormat("\n\tAccepts={0}", GetClientAcceptTypes(context));

                Platform.Log(LogLevel.Debug, info);
            }
            

        }

        /// <summary>
        /// Generates a http response based on the specified <see cref="response"/> object and send it to the client
        /// </summary>
        /// <param name="response"></param>
        /// <param name="context"></param>
        private static void SendWADOResponse(WADOResponse response, HttpListenerContext context)
        {
                
            context.Response.StatusCode = (int) HttpStatusCode.OK; // TODO: what does http protocol say about how error that occurs after OK status has been sent should  be handled?

            context.Response.ContentType = response.ContentType;

            if (response.Output == null)
            {
                context.Response.ContentLength64 = 0;

            }
            else
            {
                context.Response.ContentLength64 = response.Output.Length;
                Stream output = context.Response.OutputStream;
                output.Write(response.Output, 0, response.Output.Length);
            }

        }

       
        #endregion

        #region Public Methods

        public void Process(HttpListenerContext context)
        {
            HandleRequest(context);
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            WADORequestProcessorStatistics statistics = new WADORequestProcessorStatistics("Image Streaming");
            statistics.TotalProcessTime.Start();
            LogRequest(context);

            using(WADORequestTypeHandlerManager handlerManager = new WADORequestTypeHandlerManager())
            {
                string requestType = context.Request.QueryString["requestType"];
                IWADORequestTypeHandler typeHandler = handlerManager.GetHandler(requestType);

                WADORequestTypeHandlerContext ctx = new WADORequestTypeHandlerContext
                                                        {
                                                            HttpContext = context,
                                                            ServerAE = UriHelper.GetServerAE(context)
                                                        };

                using (WADOResponse response = typeHandler.Process(ctx))
                {
                    if (response != null)
                    {
                        statistics.TransmissionSpeed.Start();
                        SendWADOResponse(response, context);
                        statistics.TransmissionSpeed.End(); 
                        if (response.Output != null)
                        {
                            statistics.TransmissionSpeed.SetData(response.Output.Length);   
                        }
                    }    
                }
                        
            }

            statistics.TotalProcessTime.End();
            StatisticsLogger.Log(LogLevel.Debug, statistics);
            
        }

        #endregion

    }
}