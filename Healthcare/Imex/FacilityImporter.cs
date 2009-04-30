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
using System.Text;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(CsvDataImporterExtensionPoint), Name = "Facility Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class FacilityImporter : CsvDataImporterBase
    {
        private IEnumBroker _enumBroker;
        private List<InformationAuthorityEnum> _authorities;

        public FacilityImporter()
        {

        }

        /// <summary>
        /// Import external practitioner from CSV format.
        /// </summary>
        /// <param name="rows">
        /// Each string in the list must contain 4 CSV fields, as follows:
        ///     0 - Facility ID
        ///     1 - Facility Name
        ///     2 - Information Authority ID
        ///     3 - Information Authoirty Name
        /// </param>
        /// <param name="context"></param>
        public override void Import(List<string> rows, IUpdateContext context)
        {
            _enumBroker = context.GetBroker<IEnumBroker>();
            _authorities = new List<InformationAuthorityEnum>(_enumBroker.Load<InformationAuthorityEnum>(true));

            List<Facility> facilities = new List<Facility>();

            foreach (string line in rows)
            {
                // expect 4 fields in the row
                string[] fields = ParseCsv(line, 4);

                string facilityId = fields[0];
                string facilityName = fields[1];
                string informationAuthorityId = fields[2];
                string informationAuthorityName = fields[3];

                // first check if we have it in memory
                Facility facility = CollectionUtils.SelectFirst(facilities,
                    delegate(Facility f) { return f.Code == facilityId && f.Name == facilityName; });

                // if not, check the database
                if (facility == null)
                {
                    FacilitySearchCriteria where = new FacilitySearchCriteria();
                    where.Code.EqualTo(facilityId);
                    where.Name.EqualTo(facilityName);

                    IFacilityBroker broker = context.GetBroker<IFacilityBroker>();
                    facility = CollectionUtils.FirstElement(broker.Find(where));

                    // if not, create a new instance
                    if (facility == null)
                    {
                        facility = new Facility(facilityId, facilityName, GetAuthority(informationAuthorityId, informationAuthorityName));
                        context.Lock(facility, DirtyState.New);
                    }

                    facilities.Add(facility);
                }
            }
        }

        private InformationAuthorityEnum GetAuthority(string id, string name)
        {
            InformationAuthorityEnum authority = CollectionUtils.SelectFirst(_authorities,
                 delegate(InformationAuthorityEnum a) { return a.Code == id; });

            if(authority == null)
            {
                // create a new value
                InformationAuthorityEnum lastValue = CollectionUtils.LastElement(_authorities);
                authority = (InformationAuthorityEnum) _enumBroker.AddValue(typeof(InformationAuthorityEnum), id, id, name, lastValue == null ? 1 : lastValue.DisplayOrder + 1, false);
                _authorities.Add(authority);
            }
            return authority;
        }
    }
}
