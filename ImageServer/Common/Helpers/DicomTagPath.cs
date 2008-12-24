using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    [TypeConverter(typeof(DicomTagPathConverter))]
    public class DicomTagPath
    {
        private DicomTag _tag;
        private List<DicomTag> _parents;

        public DicomTag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public List<DicomTag> Parents
        {
            get { return _parents; }
            set { _parents = value; }
        }

        public string HexString()
        {
            StringBuilder tagPath = new StringBuilder();
            tagPath.Append(StringUtilities.Combine<DicomTag>(Parents, ",",
                                                             delegate(DicomTag tag) { return tag.HexString; }));
            if (tagPath.Length > 0)
                tagPath.Append(",");

            tagPath.Append(Tag.HexString);

            return tagPath.ToString();
        }

        public override string ToString()
        {
            return HexString();
        }
    }


    class DicomTagPathConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string).IsAssignableFrom(sourceType))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                DicomTagPath tagPath = new DicomTagPath();
                List<DicomTag> parents;
                DicomTag tag;
                Parse((string)value, out parents, out tag);
                tagPath.Parents = parents;
                tagPath.Tag = tag;
                return tagPath;
            }
            return base.ConvertFrom(context, culture, value);
        }

        protected static void Parse(string tagPath, out List<DicomTag> parentTags, out DicomTag tag)
        {
            Platform.CheckForNullReference(tagPath, "tagPath");

            parentTags = null;
            tag = null;

            string[] tagPathComponents = tagPath.Split(',');
            if (tagPathComponents != null)
            {
                uint tagValue;
                if (tagPathComponents.Length > 1)
                {
                    parentTags = new List<DicomTag>();

                    for (int i = 0; i < tagPathComponents.Length - 1; i++)
                    {
                        tagValue = uint.Parse(tagPathComponents[i], NumberStyles.HexNumber);
                        DicomTag parent = DicomTagDictionary.GetDicomTag(tagValue);
                        if (parent == null)
                            throw new Exception(String.Format("Specified tag {0} is not in the dictionary", parent));
                        parentTags.Add(parent);
                    }
                }

                tagValue = uint.Parse(tagPathComponents[tagPathComponents.Length - 1], NumberStyles.HexNumber);
                tag = DicomTagDictionary.GetDicomTag(tagValue);
                if (tag == null)
                    throw new Exception(String.Format("Specified tag {0} is not in the dictionary", tag));

            }
        }
    }
}