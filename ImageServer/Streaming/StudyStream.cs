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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Streaming
{
    public class StudyStream : IEnumerable<SeriesStream>
    {
        #region Private members

        private Dictionary<string, SeriesStream> _seriesList = new Dictionary<string, SeriesStream>();
        private String _studyInstanceUid = null;
        private XmlDocument _doc = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// Study Instance UID associated with this stream file.
        /// </summary>
        public String StudyInstanceUid
        {
            get
            {
                if (_studyInstanceUid == null)
                    return "";
                return _studyInstanceUid;
            }
        }

        #endregion

        #region Constructors

        public StudyStream(String studyInstanceUid)
        {
            _studyInstanceUid = studyInstanceUid;
        }

        public StudyStream()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indexer to retrieve specific <see cref="SeriesStream"/> objects from the <see cref="StudyStream"/>.
        /// </summary>
        /// <param name="seriesInstanceUid"></param>
        /// <returns></returns>
        public SeriesStream this[String seriesInstanceUid]
        {
            get
            {
                if (_seriesList.ContainsKey(seriesInstanceUid))
                    return _seriesList[seriesInstanceUid];

                return null;
            }
            set
            {
                if (value == null)
                    _seriesList.Remove(seriesInstanceUid);
                else
                {
                    _seriesList[seriesInstanceUid] = value;
                }
            }
        }

        /// <summary>
        /// Remove a specific file from the object.
        /// </summary>
        /// <param name="theFile"></param>
        /// <returns></returns>
        public bool RemoveFile(DicomFile theFile)
        {
            // Create a copy of the collection without pixel data
            DicomAttributeCollection data = theFile.DataSet;

            String studyInstanceUid = data[DicomTags.StudyInstanceUid];

            if (!_studyInstanceUid.Equals(studyInstanceUid))
                return false;
            
            String seriesInstanceUid = data[DicomTags.SeriesInstanceUid];

            SeriesStream series = this[seriesInstanceUid];

            if (series == null)
                return false;

            String sopInstanceUid = data[DicomTags.SopInstanceUid];

            InstanceStream instance = series[sopInstanceUid];
            if (instance == null)
                return false;

            // Setting the indexer to null removes the sop instance from the stream
            series[sopInstanceUid] = null;

            return true;
        }

        /// <summary>
        /// Add a <see cref="DicomFile"/> to the StudyStream.
        /// </summary>
        /// <param name="theFile"></param>
        /// <returns></returns>
        public bool AddFile(DicomFile theFile)
        {
            // Create a copy of the collection without pixel data
            DicomAttributeCollection data = theFile.DataSet.Copy(false);

            String studyInstanceUid = data[DicomTags.StudyInstanceUid];

            if (_studyInstanceUid == null)
                _studyInstanceUid = studyInstanceUid;
            else if (!_studyInstanceUid.Equals(studyInstanceUid))
            {
                Platform.Log(LogLevel.Error,
                             "Attempting to add an instance to the stream where the study instance UIDs don't match for SOP: {0}", theFile.MediaStorageSopInstanceUid);
                return false;
            }
            String seriesInstanceUid = data[DicomTags.SeriesInstanceUid];

            SeriesStream series = this[seriesInstanceUid];

            if (series == null)
            {
                series = new SeriesStream(seriesInstanceUid);
                this[seriesInstanceUid] = series;
            }

            String sopInstanceUid = data[DicomTags.SopInstanceUid];

            InstanceStream instance = series[sopInstanceUid];
            if (instance != null)
            {
                Platform.Log(LogLevel.Warn,"Attempting to add a duplicate SOP instance to the stream.  Replacing value: {0}",theFile.MediaStorageSopInstanceUid);
            }

            instance = new InstanceStream(data, theFile.SopClass, theFile.TransferSyntax);
            series[sopInstanceUid] = instance;

            return true;
        }

        /// <summary>
        /// Get an XML document representing the <see cref="StudyStream"/>.
        /// </summary>
        /// <remarks>
        /// This method can be called multiple times as DICOM SOP Instances are added
        /// to the <see cref="StudyStream"/>.  Note that caching is done of the 
        /// XmlDocument to improve performance.  If the collections in the InstanceStreams 
        /// are modified, the caching mechanism may cause the updates not to be contained
        /// in the generated XmlDocument.
        /// </remarks>
        /// <returns></returns>
        public XmlDocument GetMomento()
        {
            if (_doc == null)
                _doc = new XmlDocument();
            else
            {
                _doc.RemoveAll();
            }

            XmlElement clearCanvas = _doc.CreateElement("ClearCanvasStream");

            XmlElement study = _doc.CreateElement("Study");

            XmlAttribute studyInstanceUid = _doc.CreateAttribute("UID");
			studyInstanceUid.Value = _studyInstanceUid;
			study.Attributes.Append(studyInstanceUid);


			foreach (SeriesStream series in this)
			{
                XmlElement seriesElement = series.GetMomento(_doc);

				study.AppendChild(seriesElement);
			}

            clearCanvas.AppendChild(study);
            _doc.AppendChild(clearCanvas);

            return _doc;
        }

        /// <summary>
        /// Populate this <see cref="StudyStream"/> object based on the supplied XML document.
        /// </summary>
        /// <param name="theDocument"></param>
        public void SetMemento(XmlDocument theDocument)
        {
            if (!theDocument.HasChildNodes)
                return;

            // There should be one root node.
            XmlNode rootNode = theDocument.FirstChild;
            while (rootNode != null && !rootNode.Name.Equals("ClearCanvasStream"))
                rootNode = rootNode.NextSibling;

            if (rootNode == null)
                return;

            XmlNode studyNode = rootNode.FirstChild;

            while (studyNode != null)
            {
                // Just search for the first study node, parse it, then break
                if (studyNode.Name.Equals("Study"))
                {
                    _studyInstanceUid = studyNode.Attributes["UID"].Value;

                    if (studyNode.HasChildNodes)
                    {
                        XmlNode seriesNode = studyNode.FirstChild;

                        while (seriesNode != null)
                        {
                            String seriesInstanceUid = seriesNode.Attributes["UID"].Value;

                            SeriesStream seriesStream = new SeriesStream(seriesInstanceUid);

                            _seriesList.Add(seriesInstanceUid, seriesStream);

                            seriesStream.SetMemento(seriesNode);

                            // Go to next node in doc
                            seriesNode = seriesNode.NextSibling;
                        }
                    }
                }
                studyNode = studyNode.NextSibling;
            }
        }

        #endregion

        #region IEnumerator Implementation
        public IEnumerator<SeriesStream> GetEnumerator()
        {
            return _seriesList.Values.GetEnumerator();   
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
