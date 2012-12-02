#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using ClearCanvas.Dicom.Utilities.Xml.Nodes;


namespace ClearCanvas.Dicom.Utilities.Xml
{
    /// <summary>
    /// Class that can represent a study as XML data.
    /// </summary>
    public class StudyXml : IEnumerable<SeriesXml>
    {
        #region Private members

        private readonly Dictionary<string, SeriesXml> _seriesList = new Dictionary<string, SeriesXml>();
        private String _studyInstanceUid;
        private XmlDocument _doc;

        #endregion

        #region Public Properties

        /// <summary>
        /// Study Instance UID associated with this stream file.
        /// </summary>
        public String StudyInstanceUid
        {
            get
            {
                return _studyInstanceUid ?? string.Empty;
            }
        }

    	public String PatientsName
    	{
    		get
    		{
    		    foreach (var instance in _seriesList.Values.SelectMany(series => series))
    		    {
    		        DicomAttribute attrib;
    		        if (instance.Collection.TryGetAttribute(DicomTags.PatientsName, out attrib))
    		            return attrib.GetString(0, string.Empty);
    		    }
    		    return string.Empty;
    		}
    	}

		public String PatientId
		{
			get
			{
			    foreach (var instance in _seriesList.Values.SelectMany(series => series))
			    {
			        DicomAttribute attrib;
			        if (instance.Collection.TryGetAttribute(DicomTags.PatientId, out attrib))
			            return attrib.GetString(0, string.Empty);
			    }
			    return string.Empty;
			}
		}
		public int NumberOfStudyRelatedSeries
    	{
			get { return _seriesList.Count; }	
    	}

    	public int NumberOfStudyRelatedInstances
    	{
    		get
    		{
    		    return _seriesList.Values.Sum(series => series.NumberOfSeriesRelatedInstances);
    		}
    	}

        #endregion

        #region Constructors

        public StudyXml(String studyInstanceUid)
        {
            _studyInstanceUid = studyInstanceUid;
        }

        public StudyXml()
        {
        }

        #endregion

        #region Public Methods

        public void Unload()
        {
            foreach (var series in _seriesList.Values)
            {
                series.Unload();
            }
        }

        public bool Load(string studyInsanceUid, int numberOfStudyRelatedInstances)
        {
            var studyInfo = SeriesXml.Cache.Get(StreamCacheType.String, null, studyInsanceUid,null);
            if (studyInfo == null || studyInfo.Data == null)
                return false;

            _studyInstanceUid = studyInsanceUid;
            using (var memoryStream = new MemoryStream(studyInfo.Data))
            {
                using (var compressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(compressionStream))
                    {
                        var numberOfInstances =  int.Parse(reader.ReadToEnd());
                        if (numberOfInstances != numberOfStudyRelatedInstances)
                            return false;

                        var cachedItems = SeriesXml.Cache.EnumerateCachedItems(studyInsanceUid);
                        IList<Task> tasks = new List<Task>();
                        foreach (var seriesInstanceUid in cachedItems.Select(Path.GetFileNameWithoutExtension))
                        {
                            if (seriesInstanceUid == null)
                                return false;
                            var seriesStream = new SeriesXml(this, seriesInstanceUid);
                            string uid = seriesInstanceUid;
                            var task = Task.Factory.StartNew(() =>
                                                      {
                                                          if (seriesStream.Load())
                                                          {
                                                              lock(_seriesList)
                                                              {
                                                                  _seriesList.Add(uid, seriesStream);      
                                                              }
                                                             
                                                          }
                                                             
                                                      } );
                            tasks.Add(task);

                        }
                        Task.WaitAll(tasks.ToArray());
                        return true;
                    }
                }
            }            
        }

        /// <summary>
        /// Indexer to retrieve specific <see cref="SeriesXml"/> objects from the <see cref="StudyXml"/>.
        /// </summary>
        /// <param name="seriesInstanceUid"></param>
        /// <returns></returns>
        public SeriesXml this[String seriesInstanceUid]
        {
            get
            {
            	SeriesXml series;
				if (!_seriesList.TryGetValue(seriesInstanceUid, out series))
					return null;

            	return series;
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
            String sopInstanceUid = data[DicomTags.SopInstanceUid];

            return RemoveInstance(seriesInstanceUid, sopInstanceUid);
        }

        /// <summary>
        /// Removes a series from the StudyXml.
        /// </summary>
        /// <param name="seriesInstanceUid">The Series Instance UID of the series to be removed.</param>
        /// <returns>true if the series is removed or does not exist.</returns>
        public bool RemoveSeries(string seriesInstanceUid)
        {
            if (Contains(seriesInstanceUid))
                return _seriesList.Remove(seriesInstanceUid);
            else
                return true; // treated as ok.
        }

        /// <summary>
        /// Remove a specific SOP instance from the StudyXml.
        /// </summary>
        /// <param name="seriesInstanceUid">The Series Instance Uid of the instance to be removed</param>
        /// <param name="sopInstanceUid">The SOP Instance Uid of the instance to be removed</param>
        /// <returns>true on SOP instance exists and is removed.</returns>
        public bool RemoveInstance(String seriesInstanceUid, String sopInstanceUid)
        {
            SeriesXml series = this[seriesInstanceUid];

            if (series == null)
                return false;

            InstanceXml instance = series[sopInstanceUid];
            if (instance == null)
                return false;

            // Setting the indexer to null removes the sop instance from the stream
            series[sopInstanceUid] = null;

            return true;
        }

		/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
		/// </summary>
		/// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
		/// <returns>true on success.</returns>
		public bool AddFile(DicomFile theFile)
		{
			return AddFile(theFile, 0);
		}

		public bool AddFile(DicomFile theFile, long fileSize)
		{
			return AddFile(theFile, fileSize, new StudyXmlOutputSettings());
		}

    	/// <summary>
		/// Add a <see cref="DicomFile"/> to the StudyXml.
        /// </summary>
        /// <param name="theFile">The <see cref="DicomFile"/> to add.</param>
        /// <param name="fileSize">The size in bytes of the file being added.</param>
        /// <param name="settings">The settings used when writing out the file.</param>
        /// <returns>true on scuccess.</returns>
        public bool AddFile(DicomFile theFile, long fileSize, StudyXmlOutputSettings settings)
        {
    		Platform.CheckForNullReference(settings, "settings");

			// Create a copy of the collection without pixel data
    		var data = new InstanceXmlDicomAttributeCollection(theFile.DataSet, true,
    		                                                                        settings.IncludePrivateValues !=
    		                                                                        StudyXmlTagInclusion.IgnoreTag,
    		                                                                        settings.IncludeUnknownTags !=
    		                                                                        StudyXmlTagInclusion.IgnoreTag,
    		                                                                        DicomTags.PixelData);

            String studyInstanceUid = data[DicomTags.StudyInstanceUid];

            if (String.IsNullOrEmpty(_studyInstanceUid))
                _studyInstanceUid = studyInstanceUid;
            else if (!_studyInstanceUid.Equals(studyInstanceUid))
            {
                Platform.Log(LogLevel.Error,
                             "Attempting to add an instance to the stream where the study instance UIDs don't match for SOP: {0}",
                             theFile.MediaStorageSopInstanceUid);
                return false;
            }
            String seriesInstanceUid = data[DicomTags.SeriesInstanceUid];

            SeriesXml series = this[seriesInstanceUid];

            if (series == null)
            {
                series = new SeriesXml(this, seriesInstanceUid);
                this[seriesInstanceUid] = series;
            }

            String sopInstanceUid = data[DicomTags.SopInstanceUid];

            InstanceXml instance = series[sopInstanceUid];
            if (instance != null)
            {
                // Decided to remove this log as part of the Marmot development milestone.  Didn't seem like much value.
                //Platform.Log(LogLevel.Warn,
                //             "Attempting to add a duplicate SOP instance to the stream.  Replacing value: {0}",
                //             theFile.MediaStorageSopInstanceUid);
            }

    	    instance = new InstanceXml(data, theFile.SopClass, theFile.TransferSyntax)
    	                   {
    	                       SourceAETitle = theFile.SourceApplicationEntityTitle,
    	                       SourceFileName = theFile.Filename,
    	                       FileSize = fileSize
    	                   };
    	    series[sopInstanceUid] = instance;

            return true;
        }

        /// <summary>
        /// Gets the total size of all instances in the study.
        /// </summary>
        /// <returns>
        /// Size of the study, in bytes.
        /// </returns>
        public long GetStudySize()
        {
            long size = 0;
            foreach(SeriesXml series in this)
            {
                foreach (InstanceXml instance in series)
                    size += instance.FileSize;
            }

            return size;
        }

        /// <summary>
        /// Get an XML document representing the <see cref="StudyXml"/>.
        /// </summary>
        /// <remarks>
        /// This method can be called multiple times as DICOM SOP Instances are added
        /// to the <see cref="StudyXml"/>.  Note that caching is done of the 
        /// XmlDocument to improve performance.  If the collections in the InstanceStreams 
        /// are modified, the caching mechanism may cause the updates not to be contained
        /// in the generated XmlDocument.
        /// </remarks>
        /// <returns></returns>
        public XmlDocument GetMemento(StudyXmlOutputSettings settings)
        {
            if (_doc == null)
                _doc = new XmlDocument();
            else
            {
                _doc.RemoveAll();
            }

            XmlElement clearCanvas = _doc.CreateElement("ClearCanvasStudyXml");

            XmlElement study = _doc.CreateElement("Study");

            XmlAttribute studyInstanceUid = _doc.CreateAttribute("UID");
            studyInstanceUid.Value = _studyInstanceUid;
            study.Attributes.Append(studyInstanceUid);


            foreach (SeriesXml series in this)
            {
                XmlElement seriesElement = series.GetMemento(_doc, settings);

                study.AppendChild(seriesElement);
            }

            clearCanvas.AppendChild(study);
            _doc.AppendChild(clearCanvas);

            return _doc;
        }

        /// <summary>
        /// Populate this <see cref="StudyXml"/> object based on the supplied XML document.
        /// </summary>
        /// <param name="theDocument"></param>
        public void SetMemento(XmlDocument theDocument)
        {
            if (!theDocument.HasChildNodes)
                return;

            // There should be one root node.
            var rootNode = theDocument.FirstChild;
            while (rootNode != null && !rootNode.Name.Equals("ClearCanvasStudyXml"))
                rootNode = rootNode.NextSibling;

            if (rootNode != null)
            {
                SetMemento(new XmlNodeWrapper(rootNode));
            }
        }
        public void SetMemento(INode rootNode)
        {
            SetMemento(rootNode, false);
        }

        public void SetMemento(INode rootNode, bool isPrior)
        {

            if (rootNode == null)
                return;

            var studyEnumerator = rootNode.GetChildEnumerator();
            while (studyEnumerator.MoveNext() && studyEnumerator.Current != null)
            {
                var studyNode = studyEnumerator.Current;
                // Just search for the first study node, parse it, then break
                if (studyNode.Name.Equals("Study"))
                {
                    _studyInstanceUid = studyNode.Attribute("UID");

                    if (studyNode.HasChildNodes)
                    {
                        var seriesEnumerator = studyNode.GetChildEnumerator();
                        while (seriesEnumerator.MoveNext() && seriesEnumerator.Current != null)
                        {
                            var seriesNode = seriesEnumerator.Current;
                            var seriesInstanceUid = seriesNode.Attribute("UID");

                            var seriesStream = new SeriesXml(this, seriesInstanceUid);
                             seriesStream.SetMemento(seriesNode, isPrior);    
                            _seriesList.Add(seriesInstanceUid, seriesStream);
                        }
                    }
                }
            }
        }

        #endregion

        #region IEnumerator Implementation

        public IEnumerator<SeriesXml> GetEnumerator()
        {
            return _seriesList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public InstanceXml FindInstanceXml(string seriesUid, string instanceUid)
        {
            SeriesXml seriesXml = this[seriesUid];
            if (seriesXml == null)
                return null;

            return seriesXml[instanceUid];
        }

        /// <summary>
        /// Returns a boolean indicating whether the specified series exists in the study XML.
        /// </summary>
        /// <param name="seriesUid">The Series Instance UID of the series to check</param>
        /// <returns>True if the series exists in the study XML</returns>
        public bool Contains(string seriesUid)
        {
            return _seriesList.ContainsKey(seriesUid);
        }

        /// <summary>
        /// Returns a boolean indicating whether the specified SOP instance exists in the study XML.
        /// </summary>
        /// <param name="seriesUid">The Series Instance UID of the SOP instance to check</param>
        /// <param name="instanceUid">The SOP Instance UID of the SOP instance to check</param>
        /// <returns>True if the SOP instance exists in the study XML</returns>
        public bool Contains(string seriesUid, string instanceUid)
        {
            SeriesXml series = this[seriesUid];
            if (series==null)
            {
                return false;
            }

            return series[instanceUid] != null;
        }
    }
}