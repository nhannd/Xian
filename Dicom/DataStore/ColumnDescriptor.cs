using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
    public struct ColumnDescriptor
    {
        public ColumnDescriptor(TagName tagName, Path path, Boolean isComputed)
        {
            _tagName = tagName;
            _path = path;
            _isComputed = isComputed;
        }

        public TagName TagName
        {
            get { return _tagName; }
        }

        public Path Path
        {
            get { return _path; }
        }

        public Boolean IsComputed
        {
            get { return _isComputed; }
        }

        private TagName _tagName;
        private Path _path;
        private Boolean _isComputed;
    }
}
