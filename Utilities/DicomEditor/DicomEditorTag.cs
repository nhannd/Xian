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
        public DicomEditorTag(DicomEditorTag original)
        {
            _group = original.Group;
            _element = original.Element;
            _key = original.UidKey;
            _tagName = original.TagName;
            _vr = original.Vr;
            _length = original.Length;
            _value = original.Value;
            _parentTag = original._parentTag;
            _displayLevel = original._displayLevel;
        }

        public DicomEditorTag(ushort group, ushort element, string tagName, string vr, int length, string value, DicomEditorTag parentTag, DisplayLevel displayLevel)
        {
            _group = group;
            _element = element;            
            _tagName = tagName;
            _vr = vr;
            _length = length;
            _value = value;
            _parentTag = parentTag;
            _displayLevel = displayLevel;
            _key = this.SortKey(SortType.GroupElement);
        }

        public string UidKey
        {
            get { return this.SortKey(SortType.GroupElement); }
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
            get { return _vr; }
            set { _vr = value; }
        }

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public string Value
        {
            get { return _value; }
            set 
            { 
                _value = value;
                if (_value == null)
                {
                    this._length = 0;
                }
                else if (this._vr != "US" && this._vr != "UL")
                {
                    this._length = value.Length % 2 == 0 ? value.Length : value.Length + 1;
                }
            }
        }

        public DicomEditorTag ParentTag
        {
            get { return _parentTag; }
        }

        #region Display Items

        public string DisplayKey
        {
            get
            {
                string display = String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);

                switch (_displayLevel)
                {
                    case DisplayLevel.SequenceItemAttribute:
                        display = "      " + display;
                        break;
                    case DisplayLevel.SequenceItem:
                        display = "   " + display;
                        break;
                    case DisplayLevel.Attribute:
                        break;
                    default:
                        break;
                }

                return display;
            }
        }

        public string SortKey(SortType type)
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
                    typeSpecificModifier = _vr;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag._vr;
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Length:
                    typeSpecificModifier = _length.ToString();
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag._length.ToString();
                        parentTag = parentTag._parentTag;
                    }
                    break;
                case SortType.Value:
                    typeSpecificModifier = _value;
                    while (parentTag != null)
                    {
                        typeSpecificModifier = parentTag._value;
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

        public static string GenerateUidSearchKey(ushort group, ushort element, DicomEditorTag parent)
        {
            if (parent == null)
            {
                return String.Format("({0:x4},", group) + String.Format("{0:x4})", element);
            }
            else
            {
                return parent.SortKey(SortType.GroupElement) + String.Format("({0:x4},", group) + String.Format("{0:x4})", element);
            }           
        }

        private string _key;
        private ushort _group;
        private ushort _element;
        private string _tagName;
        private string _vr;
        private int _length;
        private string _value;
        private DisplayLevel _displayLevel;
        private DicomEditorTag _parentTag;
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
