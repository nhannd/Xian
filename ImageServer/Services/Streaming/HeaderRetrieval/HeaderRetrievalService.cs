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
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    [ServiceContract]
    public interface IHeaderRetrievalService
    {
        [OperationContract]
        Stream GetStudyHeader(string callingAETitle, HeaderRetrievalParameters parameters);
        
    }


    [ServiceBehavior(IncludeExceptionDetailInFaults = false,InstanceContextMode=InstanceContextMode.PerSession)]
    public class HeaderRetrievalService : IHeaderRetrievalService
    {
        #region IHeaderRetrievalService Members

        public Stream GetStudyHeader(string callingAETitle, HeaderRetrievalParameters parameters)
        {
            
            try
            {
                Platform.CheckForEmptyString(callingAETitle, "callingAETitle");
                Platform.CheckForNullReference(parameters, "parameters");
                Platform.CheckForEmptyString(parameters.ServerAETitle, "parameters.ServerAETitle");
                Platform.CheckForEmptyString(parameters.StudyInstanceUID, "parameters.StudyInstanceUID");
            }
            catch(ArgumentException e)
            {
                Platform.Log(LogLevel.Error, "GetStudyHeader: Error in request call:" + e.Message);
                throw;
            }

            HeaderRetrievalStatistics stats = new HeaderRetrievalStatistics();
            stats.ProcessTime.Start();

            
            HeaderLoader loader = new HeaderLoader();
                
            try
            {
                // TODO: perform permission check on callingAETitle
                
                Stream stream = loader.Load(parameters.ServerAETitle, parameters.StudyInstanceUID);
                

                return stream;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Info, "{0}: {1}", e, e.StackTrace);
                
                throw;
            }
            finally
            {
                stats.ProcessTime.End();

                if (Settings.Default.LogStatistics)
                {
                    stats.AddField("StudyInstanceUid", parameters.StudyInstanceUID);
                    
                    stats.AddSubStats("Loading", loader.Statistics);
                    StatisticsLogger.Log(LogLevel.Info, stats);    
                }
                
            }
        }

        #endregion
    }
}

