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

using System;
using System.IO;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode=InstanceContextMode.PerSession)]
    public class HeaderStreamingService : IHeaderStreamingService
    {
        #region Private Members
        private string _callerID;
        private Guid _id = Guid.NewGuid();
        #endregion

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

        #region IHeaderStreamingService Members

        public Stream GetStudyHeader(string callingAETitle, HeaderStreamingParameters parameters)
        {
            ConnectionMonitor.GetMonitor(OperationContext.Current.Host).AddContext(OperationContext.Current);
                
            HeaderStreamingStatistics stats = new HeaderStreamingStatistics();
            stats.ProcessTime.Start();

            HeaderLoader loader = null;

			try
			{
				Platform.CheckForEmptyString(callingAETitle, "callingAETitle");
				Platform.CheckForNullReference(parameters, "parameters");
				Platform.CheckForEmptyString(parameters.ReferenceID, "parameters.ReferenceID");
				Platform.CheckForEmptyString(parameters.ServerAETitle, "parameters.ServerAETitle");
				Platform.CheckForEmptyString(parameters.StudyInstanceUID, "parameters.StudyInstanceUID");

				Platform.Log(LogLevel.Debug, "Received request from {0}. Ref # {1} ", callingAETitle, parameters.ReferenceID);

				HeaderStreamingContext context = new HeaderStreamingContext();
			    context.ServiceInstanceID = ID;
				context.CallerAE = callingAETitle;
				context.Parameters = parameters;

				// TODO: perform permission check on callingAETitle

				loader = new HeaderLoader(context);
				Stream stream = loader.Load();
				if (stream == null)
					throw new FaultException(loader.FaultDescription);

                Platform.Log(LogLevel.Debug, "Response sent to {0}. Ref # {1} ", callingAETitle, parameters.ReferenceID);

				return stream;
			}
			catch (ArgumentException e)
			{
				throw new FaultException(e.Message);
			}
            catch(StudyNotFoundException e)
            {
                throw new FaultException(e.Message);
            }
            catch (StudyNotOnlineException e)
            {
                throw new FaultException(e.Message);
            }
            catch (StudyAccessException e)
            {
                throw new FaultException(e.Message);
            }
            catch (Exception e)
			{
				if (!(e is FaultException))
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