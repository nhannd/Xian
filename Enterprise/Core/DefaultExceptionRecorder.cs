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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Default implementation of <see cref="IExceptionRecorder"/>.
    /// </summary>
    public class DefaultExceptionRecorder : IExceptionRecorder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultExceptionRecorder()
        {

        }

        #region IExceptionRecorder Members

        /// <summary>
        /// Creates a <see cref="ExceptionLogEntry"/> for the specified operation and exception.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="e">The exception that was thrown.</param>
        /// <returns></returns>
        public ExceptionLogEntry CreateLogEntry(string operation, Exception e)
        {
            return new ExceptionLogEntry(operation, e, WriteXml(e));
        }

        #endregion

        private string WriteXml(Exception e)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("exception");
                WriteExceptionXml(writer, e);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                return sw.ToString();
            }
        }

        private void WriteExceptionXml(XmlWriter writer, Exception e)
        {
            writer.WriteElementString("message", e.Message);
            writer.WriteElementString("source", e.Source);
            writer.WriteStartElement("stack-trace");
            writer.WriteCData(e.StackTrace);
            writer.WriteEndElement();

            if (e.InnerException != null)
            {
                writer.WriteStartElement("inner-exception");
                WriteExceptionXml(writer, e.InnerException);
                writer.WriteEndElement();
            }
        }
    }
}
