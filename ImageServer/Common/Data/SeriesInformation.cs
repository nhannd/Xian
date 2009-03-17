using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents serializable series information.
    /// </summary>
    [XmlRoot("Series")]
    public class SeriesInformation
    {
        #region Private Members
        private string _seriesInstanceUid;
        private string _seriesDescription;
        private string _modality;
        private int _numberOfInstances = 1;
        #endregion

        #region Constructors

        public SeriesInformation()
        {
        }

        public SeriesInformation(IDicomAttributeProvider attributeProvider)
        {
            if (attributeProvider[DicomTags.SeriesInstanceUid] != null)
                SeriesInstanceUid = attributeProvider[DicomTags.SeriesInstanceUid].ToString();
            if (attributeProvider[DicomTags.SeriesDescription] != null)
                SeriesDescription = attributeProvider[DicomTags.SeriesDescription].ToString();
            if (attributeProvider[DicomTags.Modality] != null)
                Modality = attributeProvider[DicomTags.Modality].ToString();

        }

        #endregion

        #region Public Properties
        [XmlAttribute]
        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }
        [XmlAttribute]
        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }
        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        public int NumberOfInstances
        {
            get { return _numberOfInstances; }
            set { _numberOfInstances = value; }
        }

        #endregion

    }
}