using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorTag
    {
        public DicomEditorTag(DicomAttribute attribute)
            : this(attribute, null, DisplayLevel.Attribute) 
        {
        }        

        public DicomEditorTag(DicomAttribute attribute, DicomEditorTag parentTag, DisplayLevel displayLevel)
        {
            _attribute = attribute;

            _group = _attribute.Tag.Group;
            _element = _attribute.Tag.Element;
            _tagName = _attribute.Tag.Name;            

            _parentTag = parentTag;
            _displayLevel = displayLevel;
        }

        public DicomEditorTag(string group, string element, string tagName, DicomEditorTag parentTag, DisplayLevel displayLevel)
        {
            _attribute = null;

            _group = ushort.Parse(group, System.Globalization.NumberStyles.HexNumber);
            _element = ushort.Parse(element, System.Globalization.NumberStyles.HexNumber);            
            _tagName = tagName;

            _parentTag = parentTag;
            _displayLevel = displayLevel;            
        }

        public uint TagId
        {
            get { return _attribute.Tag.TagValue; } 
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
                else
                {
                    if (_attribute.Values == null)
                    {
                        return byte.Parse(_attribute.Tag.HexString.Substring(0, Math.Min(22, _attribute.Tag.HexString.Length)), System.Globalization.NumberStyles.HexNumber).ToString();
                    }
                    else
                    {
                        return _attribute.ToString();                        
                    }
                }
            }
            set 
            { 
                _attribute.SetStringValue(value);                
            }
        }

        #region Display Utilities

        public string DisplayKey
        {
            get
            {
                StringBuilder display = new StringBuilder();

                switch (_displayLevel)
                {
                    case DisplayLevel.SequenceItemAttribute:
                        display.Append("      ");
                        break;
                    case DisplayLevel.SequenceItem:
                        display.Append("   ");
                        break;
                    case DisplayLevel.Attribute:
                        break;
                    default:
                        break;
                }

                display.AppendFormat("({0:x4}, {1:x4})", _group, _element);                

                return display.ToString();
            }
        }

        public bool IsEditable()
        {
            ICollection<string> unEditableVRList = new string[] { "SQ", @"??", "OB", "OW", "UN", String.Empty };

            return !unEditableVRList.Contains(this.Vr) && !this.DisplayKey.Contains(",0000)") && !this.DisplayKey.Contains("(0002,");
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
                        return String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);
                    }
                    else
                    {
                        return _parentTag.SortKey(type) + String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);
                    }
                    break;
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
                    typeSpecificModifier = String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);
                    break;
            }

            return typeSpecificModifier + this.SortKey(SortType.GroupElement);
        }

        #endregion

        private ushort _group;
        private ushort _element;
        private string _tagName;      
        private DisplayLevel _displayLevel;
        private DicomEditorTag _parentTag;

        private DicomAttribute _attribute;
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
