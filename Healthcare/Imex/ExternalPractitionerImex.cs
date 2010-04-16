#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("ExternalPractitioner")]
	public class ExternalPractitionerImex : XmlEntityImex<ExternalPractitioner, ExternalPractitionerImex.ExternalPractitionerData>
	{
		[DataContract]
		public class ExternalPractitionerData : ReferenceEntityDataBase
		{
			[DataMember]
			public string FamilyName;

			[DataMember]
			public string GivenName;

			[DataMember]
			public string MiddleName;

			[DataMember]
			public string LicenseNumber;

			[DataMember]
			public string BillingNumber;

			/// <summary>
			/// The IsVerified property is exported and is used when importing a new practitioner.  It is not imported for existing practitioner.
			/// </summary>
			[DataMember]
			public bool IsVerified;

			/// <summary>
			/// The LastVerifiedTime is exported and is used when importing a new practitioner.  It is not imported for existing practitioner
			/// </summary>
			[DataMember]
			public DateTime? LastVerifiedTime;

			/// <summary>
			/// The LastEditedTime is exported and is used when importing a new practitioner.  It is not imported for existing practitioner
			/// </summary>
			[DataMember]
			public DateTime? LastEditedTime;

			[DataMember]
			public List<ExternalPractitionerContactPointData> ContactPoints;

			[DataMember]
			public Dictionary<string, string> ExtendedProperties;
		}

		[DataContract]
		public class ExternalPractitionerContactPointData
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string PreferredResultCommunicationMode;

			[DataMember]
			public bool IsDefaultContactPoint;

			[DataMember]
			public List<TelephoneNumberData> TelephoneNumbers;

			[DataMember]
			public List<AddressData> Addresses;

			[DataMember]
			public List<EmailAddressData> EmailAddresses;
		}

		#region Overrides

		protected override IList<ExternalPractitioner> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new ExternalPractitionerSearchCriteria();
			where.Name.FamilyName.SortAsc(0);
			where.Name.GivenName.SortAsc(1);
			where.Name.MiddleName.SortAsc(2);
			where.LicenseNumber.SortAsc(3);
			where.BillingNumber.SortAsc(4);

			return context.GetBroker<IExternalPractitionerBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override ExternalPractitionerData Export(ExternalPractitioner entity, IReadContext context)
		{
			var data = new ExternalPractitionerData
				{
					Deactivated = entity.Deactivated,
					FamilyName = entity.Name.FamilyName,
					GivenName = entity.Name.GivenName,
					MiddleName = entity.Name.MiddleName,
					LicenseNumber = entity.LicenseNumber,
					BillingNumber = entity.BillingNumber,
					IsVerified = entity.IsVerified,
					LastVerifiedTime = entity.LastVerifiedTime,
					LastEditedTime = entity.LastEditedTime,
					ContactPoints = CollectionUtils.Map(entity.ContactPoints,
						(ExternalPractitionerContactPoint cp) => new ExternalPractitionerContactPointData
							{
								Name = cp.Name,
								IsDefaultContactPoint = cp.IsDefaultContactPoint,
								PreferredResultCommunicationMode = cp.PreferredResultCommunicationMode.ToString(),
								Description = cp.Description,
								Addresses = CollectionUtils.Map(cp.Addresses, (Address a) => new AddressData(a)),
								TelephoneNumbers = CollectionUtils.Map(cp.TelephoneNumbers, (TelephoneNumber tn) => new TelephoneNumberData(tn)),
								EmailAddresses = CollectionUtils.Map(cp.EmailAddresses, (EmailAddress a) => new EmailAddressData(a))
							}),
					ExtendedProperties = ExtendedPropertyUtils.Copy(entity.ExtendedProperties)
				};

			return data;
		}

		protected override void Import(ExternalPractitionerData data, IUpdateContext context)
		{
			var prac = LoadExternalPractitioner(
				data.LicenseNumber,
				data.BillingNumber,
				context);

			var name = new PersonName(data.FamilyName, data.GivenName, data.MiddleName, null, null, null);
			if (prac == null)
			{
				// Creating a new practitioenr:  Import
				prac = new ExternalPractitioner(name, 
					data.LicenseNumber, 
					data.BillingNumber, 
					data.IsVerified, 
					data.LastVerifiedTime, 
					data.LastEditedTime, 
					new HashedSet<ExternalPractitionerContactPoint>(),
					new Dictionary<string, string>());
				context.Lock(prac, DirtyState.New);
			}
			else
			{
				prac.Name = name;
				prac.MarkEdited();
			}

			prac.Deactivated = data.Deactivated;

			if (data.ContactPoints != null)
			{
				foreach (var cpData in data.ContactPoints)
				{
					var cp = CollectionUtils.SelectFirst(prac.ContactPoints, p => p.Name == cpData.Name) ?? new ExternalPractitionerContactPoint(prac);
					UpdateExternalPractitionerContactPoint(cpData, cp);
				}
			}

			ExtendedPropertyUtils.Update(prac.ExtendedProperties, data.ExtendedProperties);
		}

		#endregion

		private static void UpdateExternalPractitionerContactPoint(ExternalPractitionerContactPointData data, ExternalPractitionerContactPoint cp)
		{
			cp.Name = data.Name;
			cp.Description = data.Description;
			cp.IsDefaultContactPoint = data.IsDefaultContactPoint;
			cp.PreferredResultCommunicationMode = (ResultCommunicationMode) Enum.Parse(typeof(ResultCommunicationMode), data.PreferredResultCommunicationMode);

			if (data.TelephoneNumbers != null)
			{
				cp.TelephoneNumbers.Clear();
				foreach (var phoneDetail in data.TelephoneNumbers)
				{
					cp.TelephoneNumbers.Add(phoneDetail.CreateTelephoneNumber());
				}
			}

			if (data.Addresses != null)
			{
				cp.Addresses.Clear();
				foreach (var addressDetail in data.Addresses)
				{
					cp.Addresses.Add(addressDetail.CreateAddress());
				}
			}

			if (data.EmailAddresses != null)
			{
				cp.EmailAddresses.Clear();
				foreach (var addressDetail in data.EmailAddresses)
				{
					cp.EmailAddresses.Add(addressDetail.CreateEmailAddress());
				}
			}
		}

		private static ExternalPractitioner LoadExternalPractitioner(string licenseNumber, string billingNumber, IPersistenceContext context)
		{
			ExternalPractitioner prac = null;

			// if either license or billing number are supplied, check for an existing practitioner
			if (!string.IsNullOrEmpty(licenseNumber) || !string.IsNullOrEmpty(billingNumber))
			{
				var criteria = new ExternalPractitionerSearchCriteria();
				if(!string.IsNullOrEmpty(licenseNumber))
					criteria.LicenseNumber.EqualTo(licenseNumber);
				if (!string.IsNullOrEmpty(billingNumber))
					criteria.BillingNumber.EqualTo(billingNumber);

				var broker = context.GetBroker<IExternalPractitionerBroker>();
				prac = CollectionUtils.FirstElement(broker.Find(criteria));
			}

			return prac;
		}
	}
}
