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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProfileTextQueryHelper : TextQueryHelper<PatientProfile, PatientProfileSearchCriteria, PatientProfileSummary>
    {
        private readonly IPersistenceContext _context;
        private readonly IPatientProfileBroker _broker;
        private readonly PatientProfileAssembler _assembler;

        public ProfileTextQueryHelper(IPersistenceContext context)
        {
            _context = context;
            _broker = _context.GetBroker<IPatientProfileBroker>();
            _assembler = new PatientProfileAssembler();
        }

        protected override PatientProfileSearchCriteria[] BuildCriteria(TextQueryRequest request)
        {
            string query = request.TextQuery;

            // this will hold all criteria
            List<PatientProfileSearchCriteria> criteria = new List<PatientProfileSearchCriteria>();

            // build criteria against names
            PersonName[] names = ParsePersonNames(query);
            criteria.AddRange(CollectionUtils.Map<PersonName, PatientProfileSearchCriteria>(names,
                delegate(PersonName n)
                {
                    PatientProfileSearchCriteria sc = new PatientProfileSearchCriteria();
                    sc.Name.FamilyName.StartsWith(n.FamilyName);
                    if (n.GivenName != null)
                        sc.Name.GivenName.StartsWith(n.GivenName);
                    return sc;
                }));

            // build criteria against Mrn identifiers
            string[] ids = ParseIdentifiers(query);
            criteria.AddRange(CollectionUtils.Map<string, PatientProfileSearchCriteria>(ids,
                         delegate(string word)
                         {
                             PatientProfileSearchCriteria c = new PatientProfileSearchCriteria();
                             c.Mrn.Id.StartsWith(word);
                             return c;
                         }));

            // build criteria against Healthcard identifiers
            criteria.AddRange(CollectionUtils.Map<string, PatientProfileSearchCriteria>(ids,
                         delegate(string word)
                         {
                             PatientProfileSearchCriteria c = new PatientProfileSearchCriteria();
                             c.Healthcard.Id.StartsWith(word);
                             return c;
                         }));


            return criteria.ToArray();
        }

        protected override bool TestSpecificity(PatientProfileSearchCriteria[] where, int threshold)
        {
            return _broker.Count(where) <= threshold;
        }

        protected override IList<PatientProfile> DoQuery(PatientProfileSearchCriteria[] where, SearchResultPage page)
        {
            return _broker.Find(where, page);
        }

        protected override PatientProfileSummary AssembleSummary(PatientProfile domainItem)
        {
            return _assembler.CreatePatientProfileSummary(domainItem, _context);
        }
    }
}
