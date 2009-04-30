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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Implements a simplified version of the HL7 XPN (Extended Person Name) data type
    /// </summary>
	public partial class PersonName : IFormattable
	{
        private void CustomInitialize()
        {
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            //TODO: honour format string

            StringBuilder sb = new StringBuilder();
            sb.Append(_familyName).Append(",");

            if (!string.IsNullOrEmpty(_prefix))
                sb.Append(" ").Append(_prefix);

            sb.Append(" ").Append(_givenName);

            if (!string.IsNullOrEmpty(_middleName))
                sb.Append(" ").Append(_middleName);

            if (!string.IsNullOrEmpty(_suffix))
                sb.Append(" ").Append(_suffix);

            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}