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
using System.Collections.Generic;
using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Mimes
{
    /// <summary>
    /// Plugin that handles wado requests that specifies "application/dicom" mime-type as the requested content type.
    /// </summary>
    [ExtensionOf(typeof(WADOMimeTypeExtensionPoint))]
    public class DefaultMimeHandler : IWADOMimeTypeHandler
    {
        #region Private Members
        private string[] _mimes = new string[] { "*/*", "application/dicom" };
        #endregion

        #region IWADOMimeTypeHandler Members

        public string[] SupportedRequestedMimeTypes
        {
            get
            {
                return _mimes;
            }
        }


        public WADOResponse GetResponseContent(HttpListenerRequest request)
        {
            WADOResponse response = new WADOResponse();
            string studyUid = request.QueryString["studyUID"];
            string seriesUid = request.QueryString["seriesUid"];
            string objectUid = request.QueryString["objectUid"];

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IReadContext ctx = store.OpenReadContext())
            {
                // TODO: Optimize this logic... we are querying the database for each requested image.
                // On the other hand, the current code can detect the situation where the study becomes locked (for update)
                // while the streaming is underway and return error to the client.
                IQueryStudyStorageLocation broker = ctx.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters parameters = new StudyStorageLocationQueryParameters();
                parameters.StudyInstanceUid = studyUid;
                IList<StudyStorageLocation> storages = broker.Execute(parameters);

                if (storages == null || storages.Count == 0)
                {
                    throw new WADOException((int)HttpStatusCode.NotFound, "Requested object doesn't exist");
                }
                else
                {
                    StudyStorageLocation storage = storages[0];
                    
                    if (storage.Lock)
                    {
                        throw new WADOException((int)HttpStatusCode.InternalServerError,
                                                    String.Format("The requested object is currently in use by another process and temporarily unavailable. Please try again later."));
                    }
                    else
                    {
                        string path = Path.Combine(storage.GetStudyPath(), seriesUid);
                        path = Path.Combine(path, objectUid + ".dcm");

                        if (!File.Exists(path))
                        {
                            throw new WADOException((int)HttpStatusCode.InternalServerError,
                                                    String.Format("Could not locate the requested object. File doesn't exist: {0}", path));
                        }
                        else
                        {
                            response.ContentType = "application/dicom";
                            response.Stream = GetContentStream(path);
                        }

                    }

                    
                }
            }

            return response;
        }

        #endregion

        private static Stream GetContentStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }

}
