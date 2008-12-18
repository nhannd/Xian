#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using System.Text.RegularExpressions;

namespace ClearCanvas.Ris.Application.Services
{
    public class TextQueryHelper
    {
        public static PersonName[] ParsePersonNames(string query)
        {
            // define a term as anything starting with a letter, and followed by letters, hyphen (-) or apostrophe (')
            // we allow hyphens for hyphenated surnames (wyatt-jones)
            // allow apostrophe for names like O'Leary
            Regex termDefinition = new Regex(@"\b[A-Za-z][A-Za-z'\-]*\b");
            string[] terms = ParseTerms(query, termDefinition);

            List<PersonName> names = new List<PersonName>();
            for (int i = 0; i < terms.Length; i++)
            {
                PersonName name = new PersonName();
                name.FamilyName = string.Join(" ", terms, 0, i + 1);
                if (i < terms.Length - 1)
                {
                    name.GivenName = string.Join(" ", terms, i + 1, terms.Length - i - 1);
                }
                names.Add(name);
            }
            return names.ToArray();
        }

        public static string[] ParseIdentifiers(string query)
        {
            // define an identifier as anything containing at least 1 digit, and any other alpha-digit chars
            Regex termDefinition = new Regex(@"\b[A-Za-z\d]*\d[A-Za-z\d]*\b");
			return ParseTerms(query, termDefinition);
        }

        public static string[] ParseTerms(string query)
        {
            Regex termDefinition = new Regex(@"\w+");
            return ParseTerms(query, termDefinition);
        }

        public static string[] ParseTerms(string query, Regex termDefinition)
        {
            MatchCollection matches = termDefinition.Matches(query);
            return CollectionUtils.Map<Match, string>(matches, delegate(Match m) { return m.Value; }).ToArray();
        }
    }

    public class TextQueryHelper<TDomainItem, TSearchCriteria, TSummary> : TextQueryHelper
        where TSearchCriteria : SearchCriteria
        where TSummary : DataContractBase
    {
        public delegate bool TestCriteriaSpecificityDelegate(TSearchCriteria[] where, int threshold);
        public delegate IList<TDomainItem> DoQueryDelegate(TSearchCriteria[] where, SearchResultPage page);

		private readonly Converter<string, TSearchCriteria[]> _buildCriteriaCallback;
		private readonly Converter<TDomainItem, TSummary> _summaryAssembler;
        private readonly DoQueryDelegate _queryCallback;
		private readonly TestCriteriaSpecificityDelegate _specificityCallback;

        /// <summary>
        /// Protected constructor for subclasses.
        /// </summary>
        protected TextQueryHelper()
        {
            
        }

        /// <summary>
        /// Public constructor allows direct use of this class without the need to create a subclass.
        /// </summary>
        /// <param name="criteriaBuilder"></param>
        /// <param name="summaryAssembler"></param>
        /// <param name="countCallback"></param>
        /// <param name="queryCallback"></param>
        public TextQueryHelper(
			Converter<string, TSearchCriteria[]> criteriaBuilder,
			Converter<TDomainItem, TSummary> summaryAssembler,
			TestCriteriaSpecificityDelegate countCallback,
			DoQueryDelegate queryCallback)
        {
            _buildCriteriaCallback = criteriaBuilder;
            _summaryAssembler = summaryAssembler;
            _specificityCallback = countCallback;
            _queryCallback = queryCallback;
        }

        public TextQueryResponse<TSummary> Query(TextQueryRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.TextQuery, "TextQuery");
            Platform.CheckArgumentRange(request.SpecificityThreshold, 0, 1000, "SpecificityThreshold");


            if (string.IsNullOrEmpty(request.TextQuery))
                return new TextQueryResponse<TSummary>(true, new List<TSummary>());


            TSearchCriteria[] where = BuildCriteria(request.TextQuery.Trim());

			// augment criteria to exclude de-activated items if specified
			if(!request.IncludeDeactivated && AttributeUtils.HasAttribute<DeactivationFlagAttribute>(typeof(TDomainItem)))
			{
				string propertyName = AttributeUtils.GetAttribute<DeactivationFlagAttribute>(typeof(TDomainItem)).PropertyName;
				SearchCondition<bool> c = new SearchCondition<bool>(propertyName);
				c.EqualTo(false);

				CollectionUtils.ForEach(where, delegate(TSearchCriteria w) { w.SubCriteria[propertyName] = c; });
			}

            // if a specificity threshold was specified, apply it now
            if (request.SpecificityThreshold > 0)
            {
                // eliminate query that would return too many results
                if (!TestSpecificity(where, request.SpecificityThreshold))
                    return new TextQueryResponse<TSummary>(true, new List<TSummary>());
            }

            // execute query
            IList<TDomainItem> matches = DoQuery(where, request.Page);

            return new TextQueryResponse<TSummary>(false,
                CollectionUtils.Map<TDomainItem, TSummary>(
                    matches,
                    delegate(TDomainItem entity)
                        {
                            return AssembleSummary(entity);
                        }));
        }

        protected virtual TSearchCriteria[] BuildCriteria(string query)
        {
            if(_buildCriteriaCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");
            return _buildCriteriaCallback(query);
        }

        protected virtual bool TestSpecificity(TSearchCriteria[] where, int threshold)
        {
            if (_specificityCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");
            return _specificityCallback(where, threshold);
        }

        protected virtual IList<TDomainItem> DoQuery(TSearchCriteria[] where, SearchResultPage page)
        {
            if (_queryCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");
            return _queryCallback(where, page);
        }

        protected virtual TSummary AssembleSummary(TDomainItem domainItem)
        {
            if(_summaryAssembler == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");
            return _summaryAssembler(domainItem);
        }

        
    }
}
