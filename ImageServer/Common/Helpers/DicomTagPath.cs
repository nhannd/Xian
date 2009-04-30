#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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