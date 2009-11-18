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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Represents a DICOM UID map 
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The original DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Source;


        /// <summary>
        /// The new DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Target;
    }

    public class StudyUidMap
    {
        /// <summary>
        /// The original DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Source;


        /// <summary>
        /// The new DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Target;


        public List<Map> Series { get; set; }
        public List<Map> Instances { get; set; }
    }

    /// <summary>
    /// Represents the Uid Map Xml used for mapping series/instance from one study to another
    /// during reconciliation.
    /// </summary>
    public class UidMapXml
    {
        [XmlArray(ElementName = "StudyUidMaps")]
        [XmlArrayItem(ElementName="Study")]
        public List<StudyUidMap> StudyUidMaps { get; set; }

        public UidMapXml()
        {
            StudyUidMaps = new List<StudyUidMap>();
        }

        /// <summary>
        /// Loads the <see cref="Series"/> and instance mappings for the specified study.
        /// </summary>
        /// <param name="location"></param>
        public void Load(StudyStorageLocation location)
        {
            Load(Path.Combine(location.GetStudyPath(), "UidMap.xml"));
        }

        /// <summary>
        /// Loads the <see cref="Series"/> and instance mappings from the specified file.
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                UidMapXml copy = XmlUtils.Deserialize<UidMapXml>(doc);
                StudyUidMaps = copy.StudyUidMaps;
            }
        }
    }
}