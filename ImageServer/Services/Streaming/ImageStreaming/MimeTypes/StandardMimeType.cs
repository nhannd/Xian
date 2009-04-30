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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class StandardMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "application/dicom"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            
            if (context.Request.QueryString["frameNumber"] != null)
            {
                // "Shall be ignored in the case of all objects other than multi-frame objects...
                // shall not be present if content type is application/dicom"
                throw new WADOException(HttpStatusCode.BadRequest,
                                        "FrameNumber shall not be present when application/dicom mime type is used. Please use a different ContentType.");
                
            }
            else
            {

                MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
                output.ContentType = OutputMimeType;
                using (FileStream stream = FileStreamOpener.OpenForRead(context.ImagePath, FileMode.Open))
                {
                    output.ContentType = OutputMimeType;
                    byte[] buffer = new byte[stream.Length];
                    int offset = 0;
                    int readBytes = 0;
                    do
                    {
                        readBytes = stream.Read(buffer, offset, buffer.Length - offset);
                        if (readBytes > 0)
                        {
                            offset += readBytes;
                        }
                    } while (readBytes > 0);
                    output.Output = buffer;
					stream.Close();
                }
                return output;    
            }
            
        }
    }

    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class JPEGMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "image/jpeg"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            throw new WADOException( HttpStatusCode.NotImplemented, "image/jpeg is not supported. Please use a different ContentType");
        }
    }
}