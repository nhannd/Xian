using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Utilities.DicomEditor
{
    public class DicomEditorTagKey
    {
        public DicomEditorTagKey(DicomEditorTagKey Key)
        {
            _group = Key._group;
            _element = Key._element;
            _parentKeyString = Key._parentKeyString;
            _displayLevel = Key._displayLevel;
        }

        public DicomEditorTagKey(ushort Group, ushort Element, string ParentKeyString, DisplayLevel DisplayLevel)
        {
            _group = Group;
            _element = Element;
            _parentKeyString = ParentKeyString;
            _displayLevel = DisplayLevel;
        }

        public string ParentKeyString
        {
            get { return _parentKeyString; }
        }

        public DisplayLevel DisplayLevel
        {
            get { return _displayLevel; }
        }

        public string DisplayKey
        {
            get {
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

        public string SortKey
        {
            get
            {
                if (_parentKeyString == null)
                {
                    return String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);
                }
                else
                {
                    return _parentKeyString.Trim() + String.Format("({0:x4},", _group) + String.Format("{0:x4})", _element);
                }
            }
        }

        public override bool Equals(object obj)
        {
            DicomEditorTagKey key = (DicomEditorTagKey)obj;
            if (this._parentKeyString == null && key._parentKeyString == null)
            {
                return (this._group == key._group && this._element == key._element);
            }

            if (this._parentKeyString == null ^ key._parentKeyString == null)
            {
                return false;
            }
            else
            {
                return (this._group == key._group && this._element == key._element) && (this._parentKeyString == key._parentKeyString);
            }
        }

        private ushort _group;
        private ushort _element;
        private string _parentKeyString;
        private DisplayLevel _displayLevel;
    }
}
