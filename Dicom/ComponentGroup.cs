#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Dicom
{
    // this class assumes that the ComponentGroup has already been decoded 
    // from any native character set into a Unicode string
    public class ComponentGroup
    {
        public ComponentGroup(string rawComponentGroupString)
        {
            _rawString = rawComponentGroupString;
            BreakApartIntoComponents();
        }

        public static ComponentGroup GetEmptyComponentGroup()
        {
            return _emptyComponentGroup;
        }

        public override string ToString()
        {
            return _rawString;
        }

        public static implicit operator String(ComponentGroup componentGroup)
        {
            return componentGroup.ToString();
        }

        protected void BreakApartIntoComponents()
        {
            string[] components = _rawString.Split('^');

            if (components.GetUpperBound(0) >= 0 && components[0] != string.Empty)
            {
                _familyName = components[0];
            }

            if (components.GetUpperBound(0) > 0 && components[1] != string.Empty)
            {
                _givenName = components[1];
            }

            if (components.GetUpperBound(0) > 1 && components[2] != string.Empty)
            {
                _middleName = components[2];
            }

            if (components.GetUpperBound(0) > 2 && components[3] != string.Empty)
            {
                _prefix = components[3];
            }

            if (components.GetUpperBound(0) > 3 && components[4] != string.Empty)
            {
                _suffix = components[4];
            }
        }

        #region Private fields
        private static ComponentGroup _emptyComponentGroup = new ComponentGroup("");
        #endregion
        #region Properties
        private string _rawString;
        private string _familyName;
        private string _givenName;
        private string _middleName;
        private string _prefix;
        private string _suffix;
	
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public string GivenName
        {
            get { return _givenName; }
            set { _givenName = value; }
        }

        public string FamilyName
        {
            get { return _familyName; }
            set { _familyName = value; }
        }


        public string RawString
        {
            get { return _rawString; }
            set { _rawString = value; }
        }
        #endregion
    }
}
