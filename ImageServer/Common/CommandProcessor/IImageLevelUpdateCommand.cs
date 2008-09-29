using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
    public interface IImageLevelUpdateCommand
    {
        ImageLevelUpdateEntry UpdateEntry { get; }
        void Apply(DicomFile file);
    }

    public class ImageLevelUpdateEntry
    {
        private DicomTag _tag;
        private List<DicomTag> _parentTags;
        private object _value;

        public DicomTag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public List<DicomTag> ParentTags
        {
            get { return _parentTags; }
            set { _parentTags = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string GetStringValue()
        {
            if (_value == null)
                return String.Empty;
            else
                return _value.ToString();
        }
    }

    
}
