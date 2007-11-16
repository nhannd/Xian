using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
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
            Regex termDefinition = new Regex(@"[A-Za-z][A-Za-z'\-]*");
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
            Regex termDefinition = new Regex(@"[A-Za-z\d]*\d[A-Za-z\d]*");
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
        where TSearchCriteria : EntitySearchCriteria
        where TSummary : DataContractBase
    {
        public delegate TSearchCriteria[] BuildCriteriaDelegate(string query);
        public delegate TSummary AssembleSummaryDelegate(TDomainItem domainItem);
        public delegate long DoCountDelegate(TSearchCriteria[] where);
        public delegate IList<TDomainItem> DoQueryDelegate(TSearchCriteria[] where, SearchResultPage page);

        private readonly BuildCriteriaDelegate _buildCriteriaCallback;
        private readonly AssembleSummaryDelegate _summaryAssembler;
        private readonly DoQueryDelegate _queryCallback;
        private readonly DoCountDelegate _countCallback;

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
        public TextQueryHelper(BuildCriteriaDelegate criteriaBuilder, AssembleSummaryDelegate summaryAssembler,
            DoCountDelegate countCallback, DoQueryDelegate queryCallback)
        {
            _buildCriteriaCallback = criteriaBuilder;
            _summaryAssembler = summaryAssembler;
            _countCallback = countCallback;
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

            // if a specificity threshold was specified, apply it now
            if (request.SpecificityThreshold > 0)
            {
                // eliminate query that would return too many results by doing a count query first
                if (DoCount(where) > request.SpecificityThreshold)
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

        protected virtual long DoCount(TSearchCriteria[] where)
        {
            if (_countCallback == null)
                throw new NotImplementedException("Method must be overridden or a delegate supplied.");
            return _countCallback(where);
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
