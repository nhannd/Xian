#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace ClearCanvas.Utilities.Manifest
{
    /// <summary>
    /// Represents a file contained in a <see cref="ClearCanvasManifest"/>.
    /// </summary>
    [XmlRoot("File")]
    public class ManifestFile
    {
        /// <summary>
        /// The relative path of the file or directory in the manifest.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The Version of the file.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The LegalCopyright of the file.
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The class used to generate the Checksum.
        /// </summary>
        public string ChecksumType { get; set; }

        /// <summary>
        /// The generated checksum.
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// The CreatedDate of the file.
        /// </summary>
        [DefaultValue(null)]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// An attribute telling if the file is optional.
        /// </summary>
        [XmlAttribute(AttributeName = "optional", DataType = "boolean")]
        [DefaultValue(false)]
        public Boolean Optional { get; set; }

        /// <summary>
        /// An attribute telling if the file should be ignored.
        /// </summary>
        [XmlAttribute(AttributeName = "ignore", DataType = "boolean")]
        [DefaultValue(false)]
        public Boolean Ignore { get; set; }

        /// <summary>
        /// Generate a checksum.
        /// </summary>
        /// <param name="fullPath">The full path of the file to generate a checksum for.</param>
        public void GenerateChecksum(string fullPath)
        {
            using (FileStream file = new FileStream(fullPath, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                Checksum = sb.ToString();
                ChecksumType = md5.GetType().ToString();
            }
        }
    }
}
