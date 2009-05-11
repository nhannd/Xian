using System;
using System.Xml.Serialization;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Represents the result of the comparison when two sets of attributes are compared using <see cref="DicomAttributeCollection.Equals()"/>.
    /// </summary>
    public class DicomAttributeComparisonResult
    {
        #region Private Memebers
        private ComparisonResultType _resultType;
        private string _tagName;
        private String _details;
        #endregion

        #region Public Properties

        /// <summary>
        /// Type of differences.
        /// </summary>
        [XmlAttribute]
        public ComparisonResultType ResultType
        {
            get { return _resultType; }
            set { _resultType = value; }
        }

        /// <summary>
        /// The name of the offending tag. This can be null if the difference is not tag specific.
        /// </summary>
        [XmlAttribute]
        public String TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        /// <summary>
        /// Detailed text describing the problem.
        /// </summary>
        public string Details
        {
            get { return _details; }
            set { _details = value; }
        }
        #endregion

    }
}