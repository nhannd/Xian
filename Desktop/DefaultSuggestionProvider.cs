using System;
using System.Collections;
using System.Text;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    public class DefaultSuggestionProvider<TItem> : ISuggestionProvider
    {
        public delegate string FormatDelegate<T>(T obj);
        public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided;

        private IList<TItem> _sourceList;
        private FormatDelegate<TItem> _formatHandler;
        private Comparison<TItem> _sortComparison;
        private List<TItem> _shortList;
        private string _lastQuery;

        public DefaultSuggestionProvider(IList<TItem> sourceList, FormatDelegate<TItem> formatHandler)
            :this(sourceList, formatHandler, null)
        {
            
        }
        public DefaultSuggestionProvider(IList<TItem> sourceList, FormatDelegate<TItem> formatHandler, Comparison<TItem> sortComparison)
        {
            _sourceList = sourceList;
            _formatHandler = formatHandler;
            _sortComparison = sortComparison;
        }

        #region ISuggestionProvider Members

        public void SetQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                _shortList = new List<TItem>();
            }
            else if (_shortList == null || string.IsNullOrEmpty(_lastQuery) || !query.StartsWith(_lastQuery))
            {
                // go back to original source
                _shortList = new List<TItem>(_sourceList);
                if(_sortComparison != null)
                    _shortList.Sort(_sortComparison);
                else 
                    _shortList.Sort();
            }


            // refine the short-list
            _shortList = _shortList.FindAll(
                delegate(TItem item)
                {
                    string itemString = _formatHandler(item);
                    return itemString.StartsWith(query, StringComparison.CurrentCultureIgnoreCase);
                });

            _lastQuery = query;

            if (SuggestionsProvided != null)
                SuggestionsProvided(this, new SuggestionsProvidedEventArgs(_shortList));

        }

        public string FormatItem(object item)
        {
            return _formatHandler((TItem)item);
        }


        #endregion
    }
}
