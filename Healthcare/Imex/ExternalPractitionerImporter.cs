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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "External Practitioner Importer")]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class ExternalPractitionerImporter : CsvDataImporterBase
	{
		private const int _numFields = 25;

		IPersistenceContext _context;

		#region CsvDataImporterBase overrides

		/// <summary>
		/// Import external practitioner from CSV format.
		/// </summary>
		/// <param name="rows">
		/// Each string in the list must contain 25 CSV fields, as follows:
		///     0 - FamilyName
		///     1 - GivenName
		///     2 - MiddleName
		///     3 - Prefix
		///     4 - Suffix
		///     5 - Degree
		///     6 - LicenseNumber
		///     7 - BillingNumber
		///     8 - Street
		///     9 - Unit
		///     10 - City
		///     11 - Province
		///     12 - PostalCode
		///     13 - Country
		///     14 - ValidFrom
		///     15 - ValidUntil
		///     16 - Phone CountryCode
		///     17 - Phone AreaCode
		///     18 - Phone Number
		///     19 - Phone Extension
		///     20 - ValidFrom
		///     21 - ValidUntil
		///     22 - Fax CountryCode
		///     23 - Fax AreaCode
		///     24 - Fax Number
		///     25 - Fax Extension
		///     26 - ValidFrom
		///     27 - ValidUntil
		/// </param>
		/// <param name="context"></param>
		public override void Import(List<string> rows, IUpdateContext context)
		{
			_context = context;

			var  importedEPs = new List<ExternalPractitioner>();
			var validator = new DomainObjectValidator();

			foreach (var row in rows)
			{
				var fields = ParseCsv(row, _numFields);

				var epFamilyName = fields[0];
				var epGivenName = fields[1];
				var epMiddlename = fields[2];
				var epPrefix = fields[3];
				var epSuffix = fields[4];
				var epDegree = fields[5];

				var epLicense = fields[6];
				var epBillingNumber = fields[7];

				var addressStreet = fields[8];
				var addressUnit = fields[9];
				var addressCity = fields[10];
				var addressProvince = fields[11];
				var addressPostalCode = fields[12];
				var addressCountry = fields[13];

				var addressValidFrom = ParseDateTime(fields[14]);
				var addressValidUntil = ParseDateTime(fields[15]);

				var phoneCountryCode = fields[16];
				var phoneAreaCode = fields[17];
				var phoneNumber = fields[18];
				var phoneExtension = fields[19];
				var phoneValidFrom = ParseDateTime(fields[20]);
				var phoneValidUntil = ParseDateTime(fields[21]);

				var faxCountryCode = fields[22];
				var faxAreaCode = fields[23];
				var faxNumber = fields[24];
				var faxExtension = fields[25];
				var faxValidFrom = ParseDateTime(fields[26]);
				var faxValidUntil = ParseDateTime(fields[27]);


				ExternalPractitioner ep = GetExternalPracitioner(epLicense, importedEPs);

				if (ep != null)
					continue;

				ep = new ExternalPractitioner {LicenseNumber = epLicense, BillingNumber = epBillingNumber};
				ep.LastEditedTime = Platform.Time;
				ep.Name = new PersonName(epFamilyName, epGivenName, epMiddlename, epPrefix, epSuffix, epDegree);

				// create a single default contact point
				var contactPoint = new ExternalPractitionerContactPoint(ep) {Name = "Default", IsDefaultContactPoint = true};

				try
				{
					var epAddress = new Address(
						addressStreet,
						addressUnit,
						addressCity,
						addressProvince,
						addressPostalCode,
						addressCountry,
						AddressType.B,
						new DateTimeRange(addressValidFrom, addressValidUntil));
					validator.Validate(epAddress);
					contactPoint.Addresses.Add(epAddress);
				}
				catch(EntityValidationException)
				{
					/* invalid address - ignore */
				}

				try
				{
					var epTelephone = new TelephoneNumber(
						phoneCountryCode,
						phoneAreaCode,
						phoneNumber,
						phoneExtension,
						TelephoneUse.WPN,
						TelephoneEquipment.PH,
						new DateTimeRange(phoneValidFrom, phoneValidUntil));

					validator.Validate(epTelephone);
					contactPoint.TelephoneNumbers.Add(epTelephone);
				}
				catch (EntityValidationException)
				{
					/* invalid phone - ignore */
				}

				try
				{
					var epFax = new TelephoneNumber(
						faxCountryCode,
						faxAreaCode,
						faxNumber,
						faxExtension,
						TelephoneUse.WPN,
						TelephoneEquipment.FX,
						new DateTimeRange(faxValidFrom, faxValidUntil));

					validator.Validate(epFax);
					contactPoint.TelephoneNumbers.Add(epFax);
				}
				catch (EntityValidationException)
				{
					/* invalid fax - ignore */
				}

				_context.Lock(ep, DirtyState.New);

				importedEPs.Add(ep);
			}
		}

		#endregion

		#region Private Methods

		private ExternalPractitioner GetExternalPracitioner(string license, IEnumerable<ExternalPractitioner> importedEPs)
		{
			// if licenseId is not supplied, then assume the record does not exist
			if (string.IsNullOrEmpty(license))
				return null;

			var externalPractitioner = CollectionUtils.SelectFirst(importedEPs, ep => Equals(ep.LicenseNumber, license));

			if (externalPractitioner == null)
			{
				var criteria = new ExternalPractitionerSearchCriteria();
				criteria.LicenseNumber.EqualTo(license);

				var broker = _context.GetBroker<IExternalPractitionerBroker>();
				externalPractitioner = CollectionUtils.FirstElement(broker.Find(criteria));
			}

			return externalPractitioner;
		}

		private static DateTime? ParseDateTime(string p)
		{
			DateTime? dt;

			try
			{
				dt = DateTime.Parse(p);
			}
			catch (Exception)
			{
				dt = null;
			}

			return dt;
		}

		#endregion
	}
}
