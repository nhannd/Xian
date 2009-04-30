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
using ClearCanvas.Common.Utilities;
using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System.Collections.Generic;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Address component
    /// </summary>
	public partial class Address : IFormattable
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Equivalence comparison which ignores validity range
        /// </summary>
        /// <param name="that">The Address to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsSameAddress(Address that)
        {
            return (that != null) &&
                ((this._unit == default(string)) ? (that._unit == default(string)) : this._unit.Equals(that._unit, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._street == default(string)) ? (that._street == default(string)) : this._street.Equals(that._street, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._city == default(string)) ? (that._city == default(string)) : this._city.Equals(that._city, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._province == default(string)) ? (that._province == default(string)) : this._province.Equals(that._province, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._postalCode == default(string)) ? (that._postalCode == default(string)) : this._postalCode.Replace(" ", "").Equals(that._postalCode.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._country == default(string)) ? (that._country == default(string)) : this._country.Equals(that._country, StringComparison.CurrentCultureIgnoreCase)) &&
                ((this._type == default(AddressType)) ? (that._type == default(AddressType)) : this._type.Equals(that._type))  &&
                true;
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO interpret the format string according to custom-defined format characters
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(_unit))
            {
                sb.Append(_unit);
                sb.Append("-");
            }
            sb.AppendFormat("{0}, {1} {2} {3}", _street, _city, _province, _postalCode);
            return sb.ToString();
        }

        #endregion

        public override string ToString()
        {
            return this.ToString(null, null);
        }
    }
}
