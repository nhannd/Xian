#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class PatientProfileTextQueryHelper : TextQueryHelper<PatientProfile, PatientProfileSearchCriteria, PatientProfileSummary>
    {
        private readonly IPersistenceContext _context;
        private readonly IPatientProfileBroker _broker;
        private readonly PatientProfileAssembler _assembler;

        public PatientProfileTextQueryHelper(IPersistenceContext context)
        {
            _context = context;
            _broker = _context.GetBroker<IPatientProfileBroker>();
            _assembler = new PatientProfileAssembler();
        }

        protected override PatientProfileSearchCriteria[] BuildCriteria(TextQueryRequest request)
        {
            string query = request.TextQuery;

            // this will hold all criteria
            var criteria = new List<PatientProfileSearchCriteria>();

            // build criteria against names
            PersonName[] names = ParsePersonNames(query);
            criteria.AddRange(CollectionUtils.Map(names,
                delegate(PersonName n)
                {
                    var sc = new PatientProfileSearchCriteria();
                    sc.Name.FamilyName.StartsWith(n.FamilyName);
                    if (n.GivenName != null)
                        sc.Name.GivenName.StartsWith(n.GivenName);
                    return sc;
                }));

            // build criteria against Mrn identifiers
            string[] ids = ParseIdentifiers(query);
            criteria.AddRange(CollectionUtils.Map(ids,
                         delegate(string word)
                         {
                             var c = new PatientProfileSearchCriteria();
                             c.Mrn.Id.StartsWith(word);
                             return c;
                         }));

            // build criteria against Healthcard identifiers
            criteria.AddRange(CollectionUtils.Map(ids,
                         delegate(string word)
                         {
                             var c = new PatientProfileSearchCriteria();
                             c.Healthcard.Id.StartsWith(word);
                             return c;
                         }));

			// sort results by patient last name (add sort directive to first instance only, otherwise we get exceptions)
        	foreach (var criterion in criteria.Take(1))
        	{
        		criterion.Name.FamilyName.SortAsc(0);
        	}


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
