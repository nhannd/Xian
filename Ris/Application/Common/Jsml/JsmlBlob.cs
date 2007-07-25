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

