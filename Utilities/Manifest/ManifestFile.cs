#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

        [XmlIgnore]
        public Boolean IsDirectory { get; set; }

        /// <summary>
        /// Generate a checksum.
        /// </summary>
        /// <param name="fullPath">The full path of the file to generate a checksum for.</param>
        public void GenerateChecksum(string fullPath)
        {
            using (FileStream file = new FileStream(fullPath, FileMode.Open,FileAccess.Read,FileShare.Read))
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

        /// <summary>
        /// Verify a checksum.
        /// </summary>
        /// <param name="fullPath">The path to the file.</param>
        public void VerifyChecksum(string fullPath)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            if (md5.GetType().ToString().Equals(ChecksumType))
            {
                using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {

                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    if (!Checksum.Equals(sb.ToString()))
                        throw new ApplicationException("Checksum does not match for file " + fullPath);
                }
            }
            else
            {
                throw new ApplicationException("Unknown checksum type: " + ChecksumType);
            }
        }
    }
}
