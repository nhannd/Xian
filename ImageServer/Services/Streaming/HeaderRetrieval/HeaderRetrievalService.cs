#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    [ServiceContract]
    public interface IHeaderRetrievalService
    {
        [OperationContract]
        [FaultContract(typeof (String))]
        Stream GetStudyHeader(string callingAETitle, HeaderRetrievalParameters parameters);
    }


    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode=InstanceContextMode.PerCall)]
    public class HeaderRetrievalService : IHeaderRetrievalService
    {
        private string _callerID;
        private Guid _id = Guid.NewGuid();

        #region Protected Properties

        /// <summary>
        /// Gets the unique ID of the service instance
        /// </summary>
        public string ID
        {
            get { return _id.ToString(); }
        }

        public string CallerID
        {
            get { return _callerID; }
            set { _callerID = value; }
        }

        #endregion

        #region IHeaderRetrievalService Members

        public Stream GetStudyHeader(string callingAETitle, HeaderRetrievalParameters parameters)
        {
            HeaderRetrievalStatistics stats = new HeaderRetrievalStatistics();
            stats.ProcessTime.Start();

            HeaderLoader loader = null;

            try
            {
                Platform.CheckForEmptyString(callingAETitle, "callingAETitle");
                Platform.CheckForNullReference(parameters, "parameters");
                Platform.CheckForEmptyString(parameters.ReferenceID, "parameters.ReferenceID");
                Platform.CheckForEmptyString(parameters.ServerAETitle, "parameters.ServerAETitle");
                Platform.CheckForEmptyString(parameters.StudyInstanceUID, "parameters.StudyInstanceUID");

                Platform.Log(LogLevel.Info, "Received request Ref # {0}", parameters.ReferenceID);

                HeaderRetrievalContext context = new HeaderRetrievalContext();
                context.ServiceInstanceID = ID;
                context.CallerAE = callingAETitle;
                context.Parameters = parameters;

                // TODO: perform permission check on callingAETitle

                loader = new HeaderLoader(context);
                if (loader.StudyExists)
                {
                    Stream stream = loader.Load();
                    Debug.Assert(stream != null);
                    return stream;
                }
                else
                {
                    throw new FaultException(
                        String.Format("Study {0} does not exist on partition {1}", parameters.StudyInstanceUID, parameters.ServerAETitle));
                }
            }
            catch (ArgumentException e)
            {
                throw new FaultException(e.Message);
            }
            catch (Exception e)
            {
                if (! (e is FaultException))
                    Platform.Log(LogLevel.Error, e, "Unable to process study header request from {0}", callingAETitle);

                throw new FaultException(e.Message);
            }
            finally
            {
                stats.ProcessTime.End();

                if (loader != null && Settings.Default.LogStatistics)
                {
                    stats.AddField("StudyInstanceUid", parameters.StudyInstanceUID);

                    stats.AddSubStats(loader.Statistics);
                    StatisticsLogger.Log(LogLevel.Info, stats);
                }
            }
        }

        #endregion
    }
}