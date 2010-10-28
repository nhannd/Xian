#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
