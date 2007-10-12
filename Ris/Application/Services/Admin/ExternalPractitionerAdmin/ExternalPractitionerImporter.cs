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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin.ExternalPractitionerAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "External Practitioner Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ExternalPractitionerImporter : DataImporterBase
    {
        private const int _numFields = 25;

        IPersistenceContext _context;

        #region DataImporterBase overrides

        public override bool SupportsCsv
        {
            get { return true; }
        }

        /// <summary>
        /// Import external practitioner from CSV format.
        /// </summary>
        /// <param name="lines">
        /// Each string in the list must contain 25 CSV fields, as follows:
        ///     0 - FamilyName
        ///     1 - GivenName
        ///     2 - MiddleName
        ///     3 - Prefix
        ///     4 - Suffix
        ///     5 - Degree
        ///     6 - LicenseNumberId
        ///     7 - LicenseNumberAssigningAuthority
        ///     8 - Street
        ///     9 - Unit
        ///     10 - City
        ///     11 - Province
        ///     12 -  PostalCode
        ///     13 - Country
        ///     14 - Type
        ///     15 - ValidFrom
        ///     16 - ValidUntil
        ///     17 - CountryCode
        ///     18 - AreaCode
        ///     19 - Number
        ///     20 - Extension
        ///     21 - Use
        ///     22 - Equipment
        ///     23 - ValidFrom
        ///     24 - ValidUntil
        /// </param>
        /// <param name="context"></param>
        public override void ImportCsv(List<string> rows, IUpdateContext context)
        {
            _context = context;

            List<ExternalPractitioner> importedEPs = new List<ExternalPractitioner>();

            foreach (string row in rows)
            {
                string[] fields = ParseCsv(row, _numFields);

                string epFamilyName = fields[0];
                string epGivenName = fields[1];
                string epMiddlename = fields[2];
                string epPrefix = fields[3];
                string epSuffix = fields[4];
                string epDegree = fields[5];

                string epLicenseId = fields[6];
                string epLicenseAssigningAuthority = fields[7];

                string addressStreet = fields[8];
                string addressUnit = fields[9];
                string addressCity = fields[10];
                string addressProvince = fields[11];
                string addressPostalCode = fields[12];
                string addressCountry = fields[13];
                AddressType addressType = TryParseOrDefault(fields[14], AddressType.B);
                DateTime? addressValidFrom = ParseDateTime(fields[15]);
                DateTime? addressValidUntil = ParseDateTime(fields[16]);

                string phoneCountryCode = fields[17];
                string phoneAreaCode = fields[18];
                string phoneNumber = fields[19];
                string phoneExtension = fields[20];
                TelephoneUse phoneUse = TryParseOrDefault(fields[21], TelephoneUse.WPN);
                TelephoneEquipment phoneEquipment = TryParseOrDefault(fields[22], TelephoneEquipment.PH);
                DateTime? phoneValidFrom = ParseDateTime(fields[23]);
                DateTime? phoneValidUntil = ParseDateTime(fields[24]);

                ExternalPractitioner ep = GetExternalPracitioner(epLicenseId, epLicenseAssigningAuthority, importedEPs);

                if (ep == null)
                {
                    ep = new ExternalPractitioner();

                    ep.LicenseNumber.Id = epLicenseId;
                    ep.LicenseNumber.AssigningAuthority = epLicenseAssigningAuthority;
                    ep.Name = new PersonName(epFamilyName, epGivenName, epMiddlename, epPrefix, epSuffix, epDegree);

                    Address epAddress = new Address(
                        addressStreet,
                        addressUnit,
                        addressCity,
                        addressProvince,
                        addressPostalCode,
                        addressCountry,
                        addressType,
                        new DateTimeRange(addressValidFrom, addressValidUntil));

                    ep.Addresses.Add(epAddress);

                    TelephoneNumber epTelephone = new TelephoneNumber(
                        phoneCountryCode,
                        phoneAreaCode,
                        phoneNumber,
                        phoneExtension,
                        phoneUse,
                        phoneEquipment,
                        new DateTimeRange(phoneValidFrom, phoneValidUntil));

                    ep.TelephoneNumbers.Add(epTelephone);

                    _context.Lock(ep, DirtyState.New);

                    importedEPs.Add(ep);
                }
            }
        }

        #endregion

        #region Private Methods

        private ExternalPractitioner GetExternalPracitioner(string licenseId, string licenseAssigningAuthority, List<ExternalPractitioner> importedEPs)
        {
            ExternalPractitioner externalPractitioner = null;

            externalPractitioner = CollectionUtils.SelectFirst<ExternalPractitioner>(importedEPs,
                delegate(ExternalPractitioner ep) { return ep.LicenseNumber.Id == licenseId && ep.LicenseNumber.AssigningAuthority == licenseAssigningAuthority; });

            if (externalPractitioner == null)
            {
                ExternalPractitionerSearchCriteria criteria = new ExternalPractitionerSearchCriteria();
                criteria.LicenseNumber.Id.EqualTo(licenseId);
                criteria.LicenseNumber.AssigningAuthority.EqualTo(licenseAssigningAuthority);

                IExternalPractitionerBroker broker = _context.GetBroker<IExternalPractitionerBroker>();
                externalPractitioner = CollectionUtils.FirstElement<ExternalPractitioner>(broker.Find(criteria));
            }

            return externalPractitioner;
        }

        private DateTime? ParseDateTime(string p)
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
