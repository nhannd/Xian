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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientProfile entity
    /// </summary>
    [UniqueKey("Mrn", new string[] { "Mrn.Id", "Mrn.AssigningAuthority" })]
	public partial class PatientProfile : Entity
	{
        private void CustomInitialize()
        {
        }

        public virtual DateTime? DateOfBirth 
        {
            get { return _dateOfBirth == null ? _dateOfBirth : _dateOfBirth.Value.Date; }
			set { _dateOfBirth = value == null ? value : value.Value.Date; } 
        }

        public virtual Address CurrentHomeAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                    delegate(Address address) { return address.Type == AddressType.R && address.IsCurrent; });
            }
        }

        public virtual Address CurrentWorkAddress
        {
            get
            {
                return CollectionUtils.SelectFirst(this.Addresses,
                   delegate(Address address) { return address.Type == AddressType.B && address.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentHomePhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                  delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.PRN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }

        public virtual TelephoneNumber CurrentWorkPhone
        {
            get
            {
                return CollectionUtils.SelectFirst(this.TelephoneNumbers,
                    delegate(TelephoneNumber phone) { return phone.Use == TelephoneUse.WPN && phone.Equipment == TelephoneEquipment.PH && phone.IsCurrent; });
            }
        }
    }
}