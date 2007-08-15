using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    /// <seealso cref="DicomFile"/>
    /// <seealso cref="DicomMessage"/>
    public abstract class DicomMessageBase
    {
        internal DicomAttributeCollection _metaInfo = null;
        internal DicomAttributeCollection _dataSet = null;

        /// <summary>
        /// The Meta information for the message.
        /// </summary>
        public DicomAttributeCollection MetaInfo
        {
            get { return _metaInfo; }
        }

        /// <summary>
        /// The DataSet for the message.
        /// </summary>
        public DicomAttributeCollection DataSet
        {
            get { return _dataSet; }
        }



    }
}