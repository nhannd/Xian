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
            _key = new DicomEditorTagKey(original.Key);
            _tagName = original.TagName;
            _vr = original.Vr;
            _length = original.Length;
            _value = original.Value;
        }

        public DicomEditorTag(ushort Group, ushort Element, string TagName, string Vr, int Length, string Value, string ParentKeyString, DisplayLevel DisplayLevel)
        {
            _group = Group;
            _element = Element;
            _key = new DicomEditorTagKey(Group, Element, ParentKeyString, DisplayLevel);
            _tagName = TagName;
            _vr = Vr;
            _length = Length;
            _value = Value;
        }

        public DicomEditorTagKey Key
        {
            get { return _key; }
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

        //public string Anonymize()
        //{
        //    //Ideally would check what Type the tag is first...
        //    this.Value = "";
        //}

        private DicomEditorTagKey _key;
        private ushort _group;
        private ushort _element;
        private string _tagName;
        private string _vr;
        private int _length;
        private string _value;
    }


}
