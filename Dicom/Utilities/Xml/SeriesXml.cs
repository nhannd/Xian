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
using System.Threading.Tasks;
using System.Xml;
using ClearCanvas.Dicom.Utilities.Xml.Nodes;
using DataCache;

namespace ClearCanvas.Dicom.Utilities.Xml
{
    /// <summary>
    /// Class that can represent a series as XML.
    /// </summary>
    public class SeriesXml : IEnumerable<InstanceXml>
    {
        #region Private members

        private Dictionary<string, InstanceXml> _sourceImageList;

        private readonly Dictionary<string, InstanceXml> _instanceList = new Dictionary<string, InstanceXml>();
        private String _seriesInstanceUid;
        private readonly string _studyInstanceUid;
        private BaseInstanceXml _seriesTagsStream;
        private bool _dirty = true; 

        #endregion

        #region Public Properties

        public String SeriesInstanceUid
        {
            get
            {
                if (_seriesInstanceUid == null)
                    return "";
                return _seriesInstanceUid;
            }
        }

    	public int NumberOfSeriesRelatedInstances
    	{
			get { return _instanceList.Count; }
    	}

        public static IUnifiedCache Cache { get; private set; }

        #endregion

        #region Constructors

        static SeriesXml()
        {
            var logger = new CacheLogger();
            Cache = new UnifiedCache(logger);     
        }

        public SeriesXml(String studyInstanceUid, String seriesInstanceUid)
        {
            _seriesInstanceUid = seriesInstanceUid;
            _studyInstanceUid = studyInstanceUid;
        }

        #endregion

        #region Public Methods

        public InstanceXml this[String sopInstanceUid]
        {
            get
            {
            	InstanceXml instance;
				if (!_instanceList.TryGetValue(sopInstanceUid, out instance))
					return null;

            	return instance;
            }
            set
            {
                if (value == null)
                    _instanceList.Remove(sopInstanceUid);
                else
                {
                    _instanceList[sopInstanceUid] = value;
                }

                _dirty = true;
            }
        }

        public void Load()
        {
             if (Cache.IsCachedToDisk(CacheType.String, _studyInstanceUid,_seriesInstanceUid))
             {
                 var item =  Cache.Get(CacheType.String, _studyInstanceUid, _seriesInstanceUid,null);
                 if (item != null && item.Data != null)
                 {
                     //unzip
                     using (var memoryStream = new MemoryStream(item.Data))
                     {
                         using (var compressionStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                         {
                             using (var reader = new StreamReader(compressionStream))
                             {
                                 SetMemento(reader.ReadToEnd());
                             }
                         }
                     } 
                 }
             }
        }
        public void Unload()
        {
            if (_seriesTagsStream != null)
                _seriesTagsStream.Unload();
            foreach (var instanceXml in _instanceList.Values)
            {
                instanceXml.Unload();
            }
        }

        #endregion

        #region Internal Methods

        internal void CalculateBaseCollectionForSeries()
        {
            if (_instanceList.Count < 2)
                return;

            // Optimization:  a base collection has already been created, just return.
            if (_seriesTagsStream != null)
                return;

            IEnumerator<InstanceXml> iterator = GetEnumerator();

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect1 = iterator.Current.Collection;

            if (false == iterator.MoveNext())
                return;

            DicomAttributeCollection collect2 = iterator.Current.Collection;

            _seriesTagsStream = new BaseInstanceXml(collect1, collect2);
        }

        internal XmlElement GetMemento(XmlDocument theDocument, StudyXmlOutputSettings settings)
        {
            _dirty = false;
            // Calc the base attributes
            CalculateBaseCollectionForSeries();

            XmlElement series = theDocument.CreateElement("Series");

            XmlAttribute seriesInstanceUid = theDocument.CreateAttribute("UID");
            seriesInstanceUid.Value = _seriesInstanceUid;
            series.Attributes.Append(seriesInstanceUid);

            XmlElement baseElement = theDocument.CreateElement("BaseInstance");

            // If there's only 1 total image in the series, leave an empty base instance
            // and just have the entire image be stored.
            if (_instanceList.Count > 1)
            {
                XmlElement baseInstance = _seriesTagsStream.GetMemento(theDocument, settings);
                baseElement.AppendChild(baseInstance);
            }
            series.AppendChild(baseElement);

            foreach (InstanceXml instance in _instanceList.Values)
            {
                instance.SetBaseInstance(_seriesTagsStream);
                XmlElement instanceElement = instance.GetMemento(theDocument, settings);

                series.AppendChild(instanceElement);
            }

            return series;
        }

        internal void SetMemento(INode theSeriesNode)
        {
            _dirty = true;
            _seriesInstanceUid = theSeriesNode.Attribute("UID");

            if (!theSeriesNode.HasChildNodes)
                return;

            var seriesXml = string.Format("<Series UID=\"{0}\">{1}</Series>", _seriesInstanceUid, theSeriesNode.InnerXml);
            //compress
 /*           Task.Factory.StartNew(() =>
                                      {*/
                                          if (!Cache.IsCachedToDisk(CacheType.String, _studyInstanceUid,_seriesInstanceUid))
                                               Cache.Put(_studyInstanceUid, _seriesInstanceUid,new StringCacheItem {Data = seriesXml});
                                     // });
            SetMemento(seriesXml);


        }
        private void SetMemento(string seriesXml)
        {
            if (String.IsNullOrEmpty(seriesXml))
                return;

            var doc = new XmlDocument();
            doc.LoadXml(seriesXml);

            var enumerator = new XmlNodeWrapper(doc.DocumentElement).GetChildEnumerator();
            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                var childNode = enumerator.Current;
                // Just search for the first study node, parse it, then break
                if (childNode.Name.Equals("BaseInstance"))
                {
                    //there should be at most one child node
                    if (childNode.HasChildNodes)
                    {
                        var instanceNode = childNode.FirstChild;
                        if (instanceNode.Name.Equals("Instance"))
                        {
                            if (_seriesTagsStream == null)
                            {
                                _seriesTagsStream = new BaseInstanceXml(this, instanceNode);                                
                            }
                            else
                            {
                                _seriesTagsStream.Load(instanceNode, null);
                            }
                        }
                    }
                }
                else if (childNode.Name.Equals("Instance"))
                {
                    // This assumes the BaseInstance is in the xml ahead of the actual instances, note, however,
                    // that if there is only 1 instance in the series, there will be no base instance value
                    string sopInstanceUid = null;
                    if (childNode.HasAttribute("UID"))
                    {
                        sopInstanceUid = childNode.Attribute("UID");
                    }
                    if (sopInstanceUid != null && _instanceList.ContainsKey(sopInstanceUid))
                    {
                        _instanceList[sopInstanceUid].Load(childNode, _seriesTagsStream);                        
                    }
                    else
                    {
                        var instanceStream = new InstanceXml(this, childNode, _seriesTagsStream);
                        _instanceList.Add(instanceStream.SopInstanceUid, instanceStream);
                    }
  
                }
            }
        }
        #endregion

        #region IEnumerator Implementation

        public IEnumerator<InstanceXml> GetEnumerator()
        {
            return _instanceList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public InstanceXml FindSourceImageInstanceXml(string instanceUid)
        {
            BuildSourceImageMap();
            InstanceXml instanceXml;
            if (_sourceImageList.TryGetValue(instanceUid, out instanceXml))
            {
                return instanceXml;
            }
            else
            {
                return null;
            }
        }

        private void BuildSourceImageMap()
        {
            if (_dirty || _sourceImageList==null)
            {
                _sourceImageList = new Dictionary<string, InstanceXml>();
                foreach(InstanceXml instanceXml in _instanceList.Values)
                {
                    if (instanceXml.SourceImageInfoList!=null)
                    {
                       foreach(SourceImageInfo sourceInfo in instanceXml.SourceImageInfoList)
                       {
                           _sourceImageList.Add(sourceInfo.SopInstanceUid, instanceXml);
                       }
                    }
                }
            }
        }
    }
}