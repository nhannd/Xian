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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;

namespace ClearCanvas.Utilities.DicomEditor.Tools
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DicomEditorCreateToolComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomEditorCreateToolComponent class
    /// </summary>
    [AssociateView(typeof(DicomEditorCreateToolComponentViewExtensionPoint))]
    public class DicomEditorCreateToolComponent : ApplicationComponent
    {
        private bool _vrEnabled;
        private bool _acceptEnabled;
        private ushort _group;
        private ushort _element;
        private string _tagName;
        private string _vr;
        private int _length;
        private string _value;

        public DicomEditorCreateToolComponent()
        {
            _vrEnabled = false;
            _acceptEnabled = false;
        }        

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public uint TagId
        {
            get { return DicomTagDictionary.GetDicomTag(_group, _element).TagValue; }
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

        public void Accept()
        {
            //check if exists
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();            
        }

        public bool AcceptEnabled
        {
            get { return _acceptEnabled; }
            set { _acceptEnabled = value; }
        }

        public void Cancel()
        {
            this.Host.Exit();
        }

        private void UpdateDialog()
        {       
            DicomTag entry = DicomTagDictionary.GetDicomTag(_group, _element);

            if (entry != null)
            {
                this.TagName = entry.Name;
                this.Vr = entry.VR.ToString();
                this.VrEnabled = false;
            }
            else
            {
                this.TagName = "Unknown";
                this.Vr = "";
                this.VrEnabled = true;
            }
      
            this.AcceptEnabled = this.AllowTagAddition();                        
        }

        private bool AllowTagAddition()
        {
            ICollection<string> badGroups = new string[] {"0000", "0001", "0003"};

            // if the group number is odd, then it's a private tag and we 
            // cannot handle private tags yet at this point, so we fail out
            // the validation
			return !(badGroups.Contains(this.Group) || this.TagName.StartsWith("Illegal") || this.Element == "0000" || (_group % 2 > 0));
        }
    }
}
