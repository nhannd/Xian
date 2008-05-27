using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for implementations of <see cref="ISuggestionProvider"/> that provides some of the boilerplate
    /// functionality.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class SuggestionProviderBase<TItem> : ISuggestionProvider
    {
        private event EventHandler<SuggestionsProvidedEventArgs> _suggestionsProvided;

        private List<TItem> _shortList;
        private string _lastQuery;

        protected SuggestionProviderBase()
        {
            // better if this is initialized to "" instead of null, so we don't have to deal further with the null case
            _lastQuery = "";
        }

        /// <summary>
        /// Called to obtain the initial source list for the specified query.  May return null if no items are available.
        /// </summary>
        /// <remarks>
        /// This method is called to obtain an initial list of items for a given query.  The method should return 
        /// null if no items are available for the specified query (or if the items are still be fetched from a remote source).
        /// This method is called repeatedly each time the user updates the query, until a non-null result is returned.
        /// Once this method returns a non-null result, it is not called again as long as subsequent queries are increasingly
        /// "specific".  The method <see cref="RefineShortList"/> is called instead.
        /// However, as soon as a less specific query is encountered (e.g. the user presses backspace), this method
        /// will be called again to generate a new source list.  It is up to the implementation to decide how/whether
        /// to cache the source list for performance considerations.
        /// </remarks>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract List<TItem> GetShortList(string query);

        /// <summary>
        /// Called to format the specified item for display in the suggestion list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract string FormatItem(TItem item);

        /// <summary>
        /// Called to successively refine a shortlist of items.
        /// </summary>
        /// <param name="shortList"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is called to refine the short-list obtained by the initial call(s) to <see cref="GetShortList"/>.
        /// The default implementation returns all items in the specified shortList that start with the specified query string.
        /// Override this method to change this behaviour.
        /// </remarks>
        protected virtual List<TItem> RefineShortList(List<TItem> shortList, string query)
        {
            // refine the short-list
            return shortList.FindAll(
                delegate(TItem item)
                {
                    string itemString = FormatItem(item);
                    return itemString.StartsWith(query, StringComparison.CurrentCultureIgnoreCase);
                });
        }

        /// <summary>
        /// Called to determine if the specified query is a refinement of the previous query,
        /// that is, whether the existing shortlist can be refined or should be discarded.
        /// </summary>
        /// <param name="newQuery"></param>
        /// <param name="previousQuery"></param>
        /// <returns></returns>
        /// <remarks>
        /// The default implementation of this method returns true if query starts with previousQuery.
        /// </remarks>
        protected virtual bool IsQueryRefinement(string query, string previousQuery)
        {
            return query.StartsWith(previousQuery);
        }

        protected void UpdateSuggestions()
        {
            // just force an update, using whatever the last known query was
            UpdateSuggestions(_lastQuery);
        }

        #region ISuggestionProvider Members

        public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided
        {
            add { _suggestionsProvided += value; }
            remove { _suggestionsProvided -= value; }
        }

        public void SetQuery(string query)
        {
            UpdateSuggestions(query);
        }

        #endregion

        private void UpdateSuggestions(string query)
        {
            // if the short list was never populated, or if the current query is not contained in the last query, start over
            if (_shortList == null || string.IsNullOrEmpty(_lastQuery) || !IsQueryRefinement(query, _lastQuery))
            {
                // go back to original source
                _shortList = GetShortList(query);
            }

            if (_shortList != null)
            {
                // refine the short-list
                _shortList = RefineShortList(_shortList, query);
            }

            _lastQuery = query;

            EventsHelper.Fire(_suggestionsProvided, this, new SuggestionsProvidedEventArgs(_shortList ?? new List<TItem>()));
        }
    }
}
