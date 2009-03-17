using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.Data
{
    /// <summary>
    /// Represents the serializable detailed information of an image set.
    /// </summary>
    [XmlRoot("Details")]
    public class ImageSetDetails
    {
        #region Private Fields
        private StudyInformation _studyInfo=new StudyInformation();
        private int _sopCount;
        #endregion

        #region Constructors

        public ImageSetDetails()
        {
        }

        public ImageSetDetails(IDicomAttributeProvider attributeProvider)
        {
            StudyInfo = new StudyInformation(attributeProvider);
            SopInstanceCount = 1;

        }

        #endregion

        #region Public Properties

        public int SopInstanceCount
        {
            get { return _sopCount; }
            set { _sopCount = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="StudyInformation"/> of the image set.
        /// </summary>
        public StudyInformation StudyInfo
        {
            get{ return _studyInfo;}
            set { _studyInfo = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Inserts a <see cref="DicomMessageBase"/> into the set.
        /// </summary>
        /// <param name="message"></param>
        public void InsertFile(DicomMessageBase message)
        {
            ImageSetDetails fileDetails = new ImageSetDetails(message.DataSet);
            StudyInfo.Add(fileDetails.StudyInfo.Series);
            SopInstanceCount++;
        }
        #endregion
    }
}