using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Streaming
{
    public class StudyStream : IEnumerable<SeriesStream>
    {
        #region Private members

        private Dictionary<String, SeriesStream> _seriesList = new Dictionary<string, SeriesStream>();
        private String _studyInstanceUid = null;

        #endregion

        #region Public Properties

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

        public SeriesStream this[String seriesInstanceUid]
        {
            get
            {
                SeriesStream series = null;
                try
                {
                    series = _seriesList[seriesInstanceUid];
                }
                catch (KeyNotFoundException) { }

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

        public bool AddFile(DicomFile theFile)
        {
            // Create a copy of the collection without pixel data
            DicomAttributeCollection data = theFile.DataSet.Copy(false);

            String studyInstanceUid = data[DicomTags.StudyInstanceUID];

            if (_studyInstanceUid == null)
                _studyInstanceUid = studyInstanceUid;
            else if (!_studyInstanceUid.Equals(studyInstanceUid))
                return false;

            String seriesInstanceUid = data[DicomTags.SeriesInstanceUID];

            SeriesStream series = this[seriesInstanceUid];

            if (series == null)
            {
                series = new SeriesStream(seriesInstanceUid);
                this[seriesInstanceUid] = series;
            }

            String sopInstanceUid = data[DicomTags.SOPInstanceUID];

            InstanceStream instance = series[sopInstanceUid];
            if (instance != null)
            {
                return false;
            }

            instance = new InstanceStream(data);
            series[sopInstanceUid] = instance;

            return true;
        }

        public XmlDocument GetMomento()
        {
            XmlDocument theDocument = new XmlDocument();

            XmlElement clearCanvas = theDocument.CreateElement("ClearCanvasStream");

            XmlElement study = theDocument.CreateElement("Study");

            XmlAttribute studyInstanceUid = theDocument.CreateAttribute("UID");
			studyInstanceUid.Value = _studyInstanceUid;
			study.Attributes.Append(studyInstanceUid);


			foreach (SeriesStream series in this)
			{
				XmlElement seriesElement = series.GetMomento(theDocument);

				study.AppendChild(seriesElement);
			}

            clearCanvas.AppendChild(study);
            theDocument.AppendChild(clearCanvas);

			return theDocument;
        }

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

                            this._seriesList.Add(seriesInstanceUid, seriesStream);

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
