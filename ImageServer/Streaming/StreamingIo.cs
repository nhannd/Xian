#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using System.IO;
using System.Xml;
using System.IO.Compression;

namespace ClearCanvas.ImageServer.Streaming
{
    public static class StreamingIo
    {
        public static void Write(XmlDocument theDoc, Stream theStream)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.Indent = false;
            xmlSettings.NewLineOnAttributes = false;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "";

            XmlWriter tw = XmlWriter.Create(theStream, xmlSettings);

            theDoc.WriteTo(tw);

            tw.Close();
        }

        public static void WriteGzip(XmlDocument theDoc, Stream theStream)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlSettings.Indent = false;
            xmlSettings.NewLineOnAttributes = false;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "";

            XmlWriter tw = XmlWriter.Create(ms, xmlSettings);

            theDoc.WriteTo(tw);

            tw.Close();

            byte[] buffer = ms.GetBuffer();

            GZipStream compressedzipStream = new GZipStream(theStream, CompressionMode.Compress, true);
            compressedzipStream.Write(buffer, 0, buffer.Length);
            // Close the stream.
            compressedzipStream.Close();
        }

        public static void ReadGzip(XmlDocument theDoc, Stream theStream)
        {
            GZipStream zipStream = new GZipStream(theStream, CompressionMode.Decompress);

            theDoc.Load(zipStream);

            return;
        }

        public static void Read(XmlDocument theDoc, Stream theStream)
        {
            theDoc.Load(theStream);
        }

    }
    
}
