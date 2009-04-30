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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class StaffDetail : DataContractBase
	{
		public StaffDetail(EntityRef staffRef, string staffId, EnumValueInfo staffType,
			PersonNameDetail personNameDetail, EnumValueInfo sex,
			string title, string licenseNumber, string billingNumber,
			List<TelephoneDetail> telephoneNumbers, List<AddressDetail> addresses, List<EmailAddressDetail> emailAddresses,
			List<StaffGroupSummary> groups, Dictionary<string, string> extendedProperties, bool deactivated, string userName)
		{
			this.StaffRef = staffRef;
			this.StaffId = staffId;
			this.StaffType = staffType;
			this.Name = personNameDetail;
			this.Sex = sex;
			this.Title = title;
			this.LicenseNumber = licenseNumber;
			this.BillingNumber = billingNumber;
			this.TelephoneNumbers = telephoneNumbers;
			this.Addresses = addresses;
			this.EmailAddresses = emailAddresses;
			this.Groups = groups;
			this.ExtendedProperties = extendedProperties;
			this.Deactivated = deactivated;
			this.UserName = userName;
		}

		public StaffDetail()
		{
			this.Name = new PersonNameDetail();
			this.EmailAddresses = new List<EmailAddressDetail>();
			this.Addresses = new List<AddressDetail>();
			this.TelephoneNumbers = new List<TelephoneDetail>();
			this.Groups = new List<StaffGroupSummary>();
			this.ExtendedProperties = new Dictionary<string, string>();
		}

		[DataMember]
		public EntityRef StaffRef;

		[DataMember]
		public string StaffId;

		[DataMember]
		public EnumValueInfo StaffType;

		[DataMember]
		public PersonNameDetail Name;

		[DataMember]
		public EnumValueInfo Sex;

		[DataMember]
		public string Title;

		[DataMember]
		public string LicenseNumber;

		[DataMember]
		public string BillingNumber;

		[DataMember]
		public List<TelephoneDetail> TelephoneNumbers;

		[DataMember]
		public List<AddressDetail> Addresses;

		[DataMember]
		public List<EmailAddressDetail> EmailAddresses;

		[DataMember]
		public List<StaffGroupSummary> Groups;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;

		[DataMember]
		public bool Deactivated;

		/// <summary>
		/// Name of associated user account.
		/// </summary>
		[DataMember]
		public string UserName;
	}
}
