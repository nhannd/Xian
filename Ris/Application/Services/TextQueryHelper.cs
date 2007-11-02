using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class TextQueryHelper
    {
        public static void SetPersonNameCriteria(PersonNameSearchCriteria criteria, IList<string> words)
        {
            if (words.Count > 0)
                criteria.FamilyName.StartsWith(words[0]);

            if (words.Count > 1)
                criteria.GivenName.StartsWith(words[1]);
        }
    }

    public class TextQueryHelper<TEntity, TSearchCriteria, TSummary> : TextQueryHelper
        where TEntity : Entity
        where TSearchCriteria : EntitySearchCriteria
        where TSummary : DataContractBase
    {
        public delegate TSearchCriteria[] BuildCriteriaDelegate(string rawQuery, List<string> terms);
        public delegate TSummary AssembleSummaryDelegate(TEntity entity);

        private readonly BuildCriteriaDelegate _buildCriteriaHandler;
        private readonly AssembleSummaryDelegate _summaryAssembler;

        public TextQueryHelper(BuildCriteriaDelegate criteriaBuilder, AssembleSummaryDelegate summaryAssembler)
        {
            _buildCriteriaHandler = criteriaBuilder;
            _summaryAssembler = summaryAssembler;
        }

        public TextQueryResponse<TSummary> Query(TextQueryRequest request, IEntityBroker<TEntity, TSearchCriteria> broker)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.TextQuery, "TextQuery");
            Platform.CheckArgumentRange(request.SpecificityThreshold, 1, 1000, "SpecificityThreshold");


            // split query on spaces or commas
            List<string> words = CollectionUtils.Map<string, string>(request.TextQuery.Split(',', ' '),
                delegate (string word)
                    {
                        return word.Trim();
                    });

            // reject empty words
            words = CollectionUtils.Reject<string>(words,
               delegate(string word)
                   {
                       return string.IsNullOrEmpty(word);
                   });

            if (words.Count == 0)
                return new TextQueryResponse<TSummary>(true, new List<TSummary>());


            TSearchCriteria[] where = _buildCriteriaHandler(request.TextQuery.Trim(), words);
            
            if (broker.Count(where) > request.SpecificityThreshold)
                return new TextQueryResponse<TSummary>(true, new List<TSummary>());

            IList<TEntity> matches = broker.Find(where);

            return new TextQueryResponse<TSummary>(false,
                CollectionUtils.Map<TEntity, TSummary>(
                    matches,
                    delegate(TEntity entity)
                        {
                            return _summaryAssembler(entity);
                        }));
        }

        
    }
}
