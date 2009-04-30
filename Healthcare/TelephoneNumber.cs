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
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// TelephoneNumber component
    /// </summary>
	public partial class TelephoneNumber
	{
        private void CustomInitialize()
        {
        }

        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }    
        }

        /// <summary>
        /// Equivalence comparison which ignores ValidRange
        /// </summary>
        /// <param name="that">The TelephoneNumber to compare to</param>
        /// <returns>True if all fields other than the validity range are the same, False otherwise</returns>
        public bool IsSameNumber(TelephoneNumber that)
        {
            return (that != null) &&
                ((this._countryCode == default(string)) ? (that._countryCode == default(string)) : this._countryCode.Equals(that._countryCode)) &&
                ((this._areaCode == default(string)) ? (that._areaCode == default(string)) : this._areaCode.Equals(that._areaCode)) &&
                ((this._number == default(string)) ? (that._number == default(string)) : this._number.Equals(that._number)) &&
                ((this._extension == default(string)) ? (that._extension == default(string)) : this._extension.Equals(that._extension)) &&
                ((this._use == default(TelephoneUse)) ? (that._use == default(TelephoneUse)) : this._use.Equals(that._use)) &&
                ((this._equipment == default(TelephoneEquipment)) ? (that._equipment == default(TelephoneEquipment)) : this._equipment.Equals(that._equipment)) &&
                true;

        }
    }
}