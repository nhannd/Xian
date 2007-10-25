using System;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// A delegate that takes the query string and return a list of suggestions
    /// </summary>
    /// <param name="query"></param>
    /// <returns>A list of suggestions</returns>
    public delegate IList SuggestionProvider(string query);

    /// <summary>
    /// Provider a few methods that optimize the query for suggestions
    /// </summary>
    public class SuggestionOptimizer
    {
        private ArrayList _suggestions;
        private string _lastQueryWithSuggestions;
        private int _minimumQueryLength = 2;

        /// <summary>
        /// Used in <see cref="GetSimipleProvider"/> to determine the minimum query string length before the inner provider is called
        /// </summary>
        public int MinimumQueryLength
        {
            get { return _minimumQueryLength; }
            set { _minimumQueryLength = value; }
        }

        /// <summary>
        /// Returns a provider that will query the inner provider only once.  This is useful for combox box with all suggestions initialized on start
        /// </summary>
        /// <param name="provider">The inner suggestion provider</param>
        /// <returns>A provider that will query the inner provider only once</returns>
        public SuggestionProvider GetSimipleProvider(SuggestionProvider provider)
        {
            if (provider == null)
                return null;

            return delegate(string query)
                       {
                           if (_suggestions == null || _suggestions.Count == 0)
                           {
                               // Perform a new query
                               ArrayList newQuery = new ArrayList();
                               newQuery.AddRange(provider(query));
                               _suggestions = newQuery;
                           }

                           return _suggestions;
                       };
        }

        /// <summary>
        /// Returns an optimized provider that query the inner provider when necessary
        /// </summary>
        /// <param name="provider">The inner suggestion provider</param>
        /// <returns>An optimized provider that query the inner provider when necessary</returns>
        public SuggestionProvider GetOptimizedProvider(SuggestionProvider provider)
        {
            if (provider == null)
                return null;

            return delegate(string query)
                       {
                           if (String.IsNullOrEmpty(query) || query.Length < _minimumQueryLength)
                               return null;

                           // if the current query starts with the last query string with suggestion, there is no need to re-query
                           // because the current suggestion list is still valid
                           if (!String.IsNullOrEmpty(_lastQueryWithSuggestions) && query.StartsWith(_lastQueryWithSuggestions))
                           {
                               return CollectionUtils.Select<object>(_suggestions,
                                   delegate(object item)
                                   {
                                       return item.ToString().StartsWith(query);
                                   });
                           }

                           // Perform a new query
                           ArrayList newQuery = new ArrayList();
                           newQuery.AddRange(provider(query));
                           if (newQuery.Count > 0)
                           {
                               _lastQueryWithSuggestions = query;
                               _suggestions = newQuery;
                           }

                           return _suggestions;
                       };
        }
    }
}
