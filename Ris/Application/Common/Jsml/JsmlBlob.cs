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
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Jsml
{
    /// <summary>
    /// Container class for a blob of JSML data.
    /// </summary>
    [DataContract]
    public class JsmlBlob
    {
        private const int ChunkSize = 8000;

        private string[] _chunks;
        private string _value;

        public JsmlBlob()
        {
        }

        public JsmlBlob(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the JSML blob as a string.
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the JSML blob as an array of chunks, for serialization
        /// </summary>
        [DataMember]
        private string[] Chunks
        {
            get { return _chunks; }
            set { _chunks = value; }
        }

        /// <summary>
        /// Breaks up the JSML blob into chunks, which avoids having to increase WCF string-length quotas at the binding level.
        /// </summary>
        /// <param name="context"></param>
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (_value != null)
            {
                int c = 0;
                List<string> pieces = new List<string>();
                while (c < _value.Length)
                {
                    pieces.Add(_value.Substring(c, Math.Min(ChunkSize, _value.Length - c)));
                    c += ChunkSize;
                }
                _chunks = pieces.ToArray();
            }
        }

        /// <summary>
        /// Reconstitutes the JSML blob from the chunks, which avoids having to increase WCF string-length quotas at the binding level.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_chunks == null)
                return;

            StringBuilder sb = new StringBuilder();
            foreach (string chunk in _chunks)
                sb.Append(chunk);

            _value = sb.ToString();

            // release chunks
            _chunks = null;
        }
    }
}

