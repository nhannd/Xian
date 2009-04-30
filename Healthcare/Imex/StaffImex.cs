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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Staff")]
	public class StaffImex : XmlEntityImex<Staff, StaffImex.StaffData>
	{
		[DataContract]
		public class StaffData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string FamilyName;

			[DataMember]
			public string GivenName;

			[DataMember]
			public string MiddleName;

			[DataMember]
			public string Sex;

			[DataMember]
			public string Title;

			[DataMember]
			public string StaffType;

			[DataMember]
			public string UserName;

			[DataMember]
			public string LicenseNumber;

			[DataMember]
			public string BillingNumber;

			[DataMember]
			public List<TelephoneNumberData> TelephoneNumbers;

			[DataMember]
			public List<AddressData> Addresses;

			[DataMember]
			public List<EmailAddressData> EmailAddresses;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;
		}


		#region Overrides

		protected override IList<Staff> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			StaffSearchCriteria where = new StaffSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IStaffBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override StaffData Export(Staff entity, IReadContext context)
		{
			StaffData data = new StaffData();
			data.Deactivated = entity.Deactivated;
			data.Id = entity.Id;
			data.StaffType = entity.Type.Code;
			data.Title = entity.Title;
			data.FamilyName = entity.Name.FamilyName;
			data.GivenName = entity.Name.GivenName;
			data.MiddleName = entity.Name.MiddleName;
			data.Sex = entity.Sex.ToString();
			data.UserName = entity.UserName;
			data.LicenseNumber = entity.LicenseNumber;
			data.BillingNumber = entity.BillingNumber;

			data.TelephoneNumbers = CollectionUtils.Map<TelephoneNumber, TelephoneNumberData>(entity.TelephoneNumbers,
				delegate(TelephoneNumber tn) { return new TelephoneNumberData(tn); });
			data.Addresses = CollectionUtils.Map<Address, AddressData>(entity.Addresses,
				delegate(Address a) { return new AddressData(a); });
			data.EmailAddresses = CollectionUtils.Map<EmailAddress, EmailAddressData>(entity.EmailAddresses,
				delegate(EmailAddress a) { return new EmailAddressData(a); });

			data.ExtendedProperties = new Dictionary<string, string>(entity.ExtendedProperties);

			return data;
		}

		protected override void Import(StaffData data, IUpdateContext context)
		{
			Staff staff = GetStaff(data.Id, context);
			staff.Deactivated = data.Deactivated;
			staff.Type = context.GetBroker<IEnumBroker>().Find<StaffTypeEnum>(data.StaffType);
			staff.Title = data.Title;
			staff.Name.FamilyName = data.FamilyName;
			staff.Name.GivenName = data.GivenName;
			staff.Name.MiddleName = data.MiddleName;

			staff.Sex = string.IsNullOrEmpty(data.Sex) == false
				? (Sex)Enum.Parse(typeof(Sex), data.Sex)
				: Sex.U;

			staff.UserName = data.UserName;
			staff.LicenseNumber = data.LicenseNumber;
			staff.BillingNumber = data.BillingNumber;

			if (data.TelephoneNumbers != null)
			{
				staff.TelephoneNumbers.Clear();
				foreach (TelephoneNumberData phoneDetail in data.TelephoneNumbers)
				{
					staff.TelephoneNumbers.Add(phoneDetail.CreateTelephoneNumber());
				}
			}

			if (data.Addresses != null)
			{
				staff.Addresses.Clear();
				foreach (AddressData address in data.Addresses)
				{
					staff.Addresses.Add(address.CreateAddress());
				}
			}

			if (data.EmailAddresses != null)
			{
				staff.EmailAddresses.Clear();
				foreach (EmailAddressData addressDetail in data.EmailAddresses)
				{
					staff.EmailAddresses.Add(addressDetail.CreateEmailAddress());
				}
			}

			if (data.ExtendedProperties != null)
			{
				foreach (KeyValuePair<string, string> kvp in data.ExtendedProperties)
				{
					staff.ExtendedProperties[kvp.Key] = kvp.Value;
				}
			}
		}

		#endregion


		private Staff GetStaff(string id, IPersistenceContext context)
		{
			Staff staff = null;

			try
			{
				StaffSearchCriteria criteria = new StaffSearchCriteria();
				criteria.Id.EqualTo(id);

				IStaffBroker broker = context.GetBroker<IStaffBroker>();
				staff = broker.FindOne(criteria);
			}
			catch (EntityNotFoundException)
			{
				staff = new Staff();

				// need to populate required fields before we can lock (use dummy values)
				staff.Id = id;
				staff.Name.FamilyName = "";
				staff.Name.GivenName = "";
				staff.Sex = Sex.U;

				context.Lock(staff, DirtyState.New);
			}
			return staff;
		}
	}
}
