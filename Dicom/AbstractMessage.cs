using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom
{
    /// <summary>
    /// Base class for DICOM Files and Messages
    /// </summary>
    public abstract class AbstractMessage
    {
        internal AttributeCollection _metaInfo = null;
        internal AttributeCollection _dataSet = null;

        public AttributeCollection MetaInfo
        {
            get { return _metaInfo; }
        }
        public AttributeCollection DataSet
        {
            get { return _dataSet; }
        }



    }
}