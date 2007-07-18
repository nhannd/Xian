using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    public abstract class DicomMessageBase
    {
        internal DicomAttributeCollection _metaInfo = null;
        internal DicomAttributeCollection _dataSet = null;

        public DicomAttributeCollection MetaInfo
        {
            get { return _metaInfo; }
        }
        public DicomAttributeCollection DataSet
        {
            get { return _dataSet; }
        }



    }
}