using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

using ClearCanvas.Dicom.OffisWrapper;
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomEditorCreateToolComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomEditorCreateToolComponent class
    /// </summary>
    [AssociateView(typeof(DicomEditorCreateToolComponentViewExtensionPoint))]
    public class DicomEditorCreateToolComponent : ApplicationComponent
    {
        private bool _vrEnabled;
        private string[] _vrList;
        private ushort _group;
        private ushort _element;
        private string _tagName;
        private string _vr;
        private int _length;
        private string _value;

        public DicomEditorCreateToolComponent()
        {
            _vrEnabled = false;

            DcmVR tempVr;
            List<string> list = new List<string>();
            foreach (DcmEVR evr in System.Enum.GetValues(typeof(DcmEVR)))
            {
                tempVr = new DcmVR(evr);
                list.Add(tempVr.getValidVRName());
            }
            _vrList = list.ToArray();
        }        

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public DicomEditorTag Tag
        {
            get { return new DicomEditorTag(_group, _element, _tagName, _vr, _length, _value, null, DisplayLevel.Attribute); }
        }

        public string Group
        {
            get { return String.Format("{0:x4}", _group); }
            set 
            {
                _group = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber);
                UpdateDialog();
            }
        }

        public string Element
        {
            get { return String.Format("{0:x4}", _element); }
            set 
            {
                _element = ushort.Parse(value, System.Globalization.NumberStyles.HexNumber);
                UpdateDialog();
            }
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

        public string Value
        {
            get { return _value; }
            set 
            {
                _value = value;
                if (_vr != "US" && _vr != "UL")
                {
                    _length = _value.Length % 2 == 0 ? _value.Length : _value.Length + 1;
                }
                else if (_vr == "US")
                {
                    _length = 2;
                }
                else if (_vr == "UL")
                {
                    _length = 4;
                }
            }
        }

        public bool VrEnabled
        {
            get { return _vrEnabled; }
            set { _vrEnabled = value; }
        }

        public string[] VrList
        {
            get { return _vrList; }
        }

        public void Accept()
        {
            //check if exists
            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }

        private void UpdateDialog()
        {       
            DcmDataDictionary dictionary = new DcmDataDictionary(true, false);
            DcmDictEntry entry = dictionary.findEntry(new DcmTagKey(_group, _element), null);
            
            if (entry != null)
            {
                this.TagName = entry.getTagName();
                this.Vr = entry.getVR().getVRName();
                this.VrEnabled = false;
            }
            else
            {
                this.TagName = "Unknown";
                this.Vr = "";
                this.VrEnabled = true;
            }
        }
    }
}
