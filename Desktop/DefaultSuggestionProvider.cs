using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A default implementation of <see cref="ISuggestionProvider"/>, dynamically providing suggested
	/// text based on user input.
	/// </summary>
	/// <typeparam name="TItem">The type of object for which suggestions should be made.</typeparam>
    public class DefaultSuggestionProvider<TItem> : ISuggestionProvider
    {
        /// <summary>
        /// Gets a formatted string based on the input item, for use in determining suggestions.
        /// </summary>
		public delegate string FormatDelegate<T>(T obj);

		/// <summary>
		/// Notifies the user-interfaces that an updated list of suggestions is available.
		/// </summary>
		public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided;

        private IList<TItem> _sourceList;
        private FormatDelegate<TItem> _formatHandler;
        private Comparison<TItem> _sortComparison;
        private List<TItem> _shortList;
        private string _lastQuery;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceList">The source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
        public DefaultSuggestionProvider(IList<TItem> sourceList, FormatDelegate<TItem> formatHandler)
            :this(sourceList, formatHandler, null)
        {
            
        }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceList">The source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		/// <param name="sortComparison">A <see cref="Comparison{T}"/> object used to sort the list of suggestions.</param>
		public DefaultSuggestionProvider(IList<TItem> sourceList, FormatDelegate<TItem> formatHandler, Comparison<TItem> sortComparison)
        {
            _sourceList = sourceList;
            _formatHandler = formatHandler;
            _sortComparison = sortComparison;
        }

        #region ISuggestionProvider Members

		/// <summary>
		/// Called by the user-inteface to inform this object of changes in the user query text.
		/// </summary>
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

        #endregion
    }
}
