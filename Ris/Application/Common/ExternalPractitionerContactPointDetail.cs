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
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ExternalPractitionerContactPointDetail : DataContractBase, ICloneable
	{
		public ExternalPractitionerContactPointDetail(EntityRef contactPointRef, string name, string description, bool isDefaultContactPoint, 
			EnumValueInfo preferredResultCommunicationMode, List<TelephoneDetail> phoneDetails, List<AddressDetail> addressDetails,
			TelephoneDetail currentPhone, TelephoneDetail currentFax, AddressDetail currentAddress, bool deactivated)
		{
			this.ContactPointRef = contactPointRef;
			this.Name = name;
			this.Description = description;
			this.IsDefaultContactPoint = isDefaultContactPoint;
			this.PreferredResultCommunicationMode = preferredResultCommunicationMode;
			this.TelephoneNumbers = phoneDetails;
			this.Addresses = addressDetails;
			this.CurrentPhoneNumber = currentPhone;
			this.CurrentFaxNumber = currentFax;
			this.CurrentAddress = currentAddress;
			this.Deactivated = deactivated;
		}

		public ExternalPractitionerContactPointDetail()
		{
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Addresses = new List<AddressDetail>();
		}

		[DataMember]
		public EntityRef ContactPointRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public bool IsDefaultContactPoint;

		[DataMember]
		public EnumValueInfo PreferredResultCommunicationMode;

		[DataMember]
		public TelephoneDetail CurrentPhoneNumber;

		[DataMember]
		public TelephoneDetail CurrentFaxNumber;

		[DataMember]
		public AddressDetail CurrentAddress;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public bool Deactivated;

		public ExternalPractitionerContactPointSummary GetSummary()
		{
			return new ExternalPractitionerContactPointSummary(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				this.Deactivated);
		}

		#region ICloneable Members

		public object Clone()
		{
			return new ExternalPractitionerContactPointDetail(
				this.ContactPointRef,
				this.Name,
				this.Description,
				this.IsDefaultContactPoint,
				this.PreferredResultCommunicationMode,
				CollectionUtils.Map(this.TelephoneNumbers, (TelephoneDetail detail) => (TelephoneDetail) detail.Clone()),
				CollectionUtils.Map(this.Addresses, (AddressDetail detail) => (AddressDetail) detail.Clone()),
				(TelephoneDetail)(this.CurrentPhoneNumber == null ? null : this.CurrentPhoneNumber.Clone()),
				(TelephoneDetail) (this.CurrentFaxNumber == null ? null : this.CurrentFaxNumber.Clone()),
				(AddressDetail) (this.CurrentAddress == null ? null : this.CurrentAddress.Clone()),
				this.Deactivated
			);
		}

		#endregion
	}
}
