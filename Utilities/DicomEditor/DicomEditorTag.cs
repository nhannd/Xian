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
using System.Text;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorTag
    {
        public DicomEditorTag(DicomAttribute attribute)
            : this(attribute, null, 0) 
        {
        }        

        public DicomEditorTag(DicomAttribute attribute, DicomEditorTag parentTag, int nestingLevel)
        {
            _attribute = attribute;

            _group = _attribute.Tag.Group;
            _element = _attribute.Tag.Element;
            _tagName = _attribute.Tag.Name;            

            _parentTag = parentTag;
            _nestingLevel = nestingLevel;
            _postitionOrdinal = 0;
        }

        public DicomEditorTag(string group, string element, string tagName, DicomEditorTag parentTag, int positionOrdinal, int displayLevel)
        {
            _attribute = null;

            _group = ushort.Parse(group, System.Globalization.NumberStyles.HexNumber);
            _element = ushort.Parse(element, System.Globalization.NumberStyles.HexNumber);            
            _tagName = tagName;

            _parentTag = parentTag;
            _postitionOrdinal = positionOrdinal;
            _nestingLevel = displayLevel;            
        }

        public uint TagId
        {
            get { return _attribute == null ? 0 : _attribute.Tag.TagValue; } 
        }

        public ushort Group
        {
            get { return _group; }
        }

        public ushort Element
        {
            get { return _element; }
        }

        public string TagName
        {
            get { return _tagName; }
            set { _tagName = value; }
        }

        public string Vr
        {
            get { return _attribute == null? String.Empty : _attribute.Tag.VR.Name; }
        }

        public string Length
        {
            get { return _attribute == null? String.Empty : _attribute.StreamLength.ToString(); }            
        }

        public string Value
        {
            get 
            {
                if (_attribute == null)
                    return String.Empty;
                else if (this.Vr == "OB" && _attribute.StreamLength > 22)
                {
                    return "<OB Data>";
                }
                else
                {
                    return _attribute.ToString();                        
                }
            }
            set 
            { 
                if (this.IsEditable())
                    _attribute.SetStringValue(value);                
            }
        }

        #region Display Utilities

        public string DisplayKey
        {
            get
            {
                StringBuilder display = new StringBuilder();

                for (int i = 0; i < _nestingLevel; i++)
                {
                    display.Append("      ");
                }                
                display.AppendFormat("({0:x4}, {1:x4})", _group, _element);                

                return display.ToString();
            }
        }

        public bool IsEditable()
        {
            return !_unEditableVRList.Contains(this.Vr) && !this.DisplayKey.Contains(",0000)") && !this.DisplayKey.Contains("(0002,");
        }

        public static int TagCompare(DicomEditorTag one, DicomEditorTag two, SortType type)
        {
            return one.SortKey(type).CompareTo(two.SortKey(type));
        }

        private string SortKey(SortType type)
        {
            DicomEditorTag parentTag = _parentTag;
            string typeSpecificModifier;

            switch (type)
            {
                case SortType.GroupElement:
                    if (_parentTag == null)
                    {
                        return String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    }
                    else
                    {
                        return _parentTag.SortKey(type) + String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    }
                case SortType.TagName:
                    typeSpecificModifier = _tagName;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag._tagName;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Vr:
                    typeSpecificModifier = this.Vr;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Vr;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Length:
                    typeSpecificModifier = this.Length;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Length;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Value:
                    typeSpecificModifier = this.Value;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag.Value;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                default:
                    typeSpecificModifier = String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element) + _postitionOrdinal.ToString();
                    break;
            }

            return typeSpecificModifier + this.SortKey(SortType.GroupElement);
        }

        #endregion

        private ushort _group;
        private ushort _element;
        private string _tagName;      
        private int _nestingLevel;
        private int _postitionOrdinal;
        private DicomEditorTag _parentTag;

        private DicomAttribute _attribute;
        private ICollection<string> _unEditableVRList = new string[] { "SQ", @"??", "OB", "OW", "UN", String.Empty };
    }

    public enum SortType
    {
        GroupElement,
        TagName,
        Vr,
        Length,
        Value
    }
}
