#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A default implementation of <see cref="ISuggestionProvider"/>, dynamically providing suggested
	/// text based on user input.
	/// </summary>
	/// <typeparam name="TItem">The type of object for which suggestions should be made.</typeparam>
    public class DefaultSuggestionProvider<TItem> : SuggestionProviderBase<TItem>
    {
        private readonly Converter<TItem, string> _formatHandler;
		private readonly Converter<string, IList<TItem>> _shortlistProvider;


		/// <summary>
		/// Constructor that accepts the full list of possible items.
		/// </summary>
		/// <param name="sourceList">The sorted source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		public DefaultSuggestionProvider(IList<TItem> sourceList, Converter<TItem, string> formatHandler)
		{
			_shortlistProvider = delegate(string q) { return string.IsNullOrEmpty(q) ? null : sourceList; };
			_formatHandler = formatHandler;
			this.AutoSort = false;	// assume the source list is already sorted
		}

		/// <summary>
		/// Constructor that accepts the full list of possible items.
		/// </summary>
		/// <param name="sourceList">The sorted source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		/// <param name="refinementStrategy">An object that specifies how the shortlist is refined in response to user input.</param>
		public DefaultSuggestionProvider(IList<TItem> sourceList, Converter<TItem, string> formatHandler, IRefinementStrategy refinementStrategy)
			:base(refinementStrategy)
        {
			_shortlistProvider = delegate(string q) { return string.IsNullOrEmpty(q) ? null : sourceList; };
            _formatHandler = formatHandler;
			this.AutoSort = false;	// assume the source list is already sorted
		}

		/// <summary>
		/// Constructor that accepts a delegate for providing a shortlist on demand.
		/// </summary>
		/// <param name="shortlistProvider">A delegate that obtains the shortlist for a specified query, or null to indicate that it should be called again.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		public DefaultSuggestionProvider(Converter<string, IList<TItem>> shortlistProvider, Converter<TItem, string> formatHandler)
		{
			_shortlistProvider = shortlistProvider;
			_formatHandler = formatHandler;
		}

		/// <summary>
		/// Constructor that accepts a delegate for providing a shortlist on demand.
		/// </summary>
		/// <param name="shortlistProvider">A delegate that obtains the shortlist for a specified query, or null to indicate that it should be called again.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		/// <param name="refinementStrategy">An object that specifies how the shortlist is refined in response to user input.</param>
		public DefaultSuggestionProvider(Converter<string, IList<TItem>> shortlistProvider, Converter<TItem, string> formatHandler, IRefinementStrategy refinementStrategy)
			:base(refinementStrategy)
		{
			_shortlistProvider = shortlistProvider;
			_formatHandler = formatHandler;
		}

		/// <summary>
		/// Called to obtain the initial source list for the specified query.  May return null if no items are available.
		/// </summary>
		protected override IList<TItem> GetShortList(string query)
        {
			return _shortlistProvider(query);
        }

		/// <summary>
		/// Called to format the specified item for display in the suggestion list.
		/// </summary>
		protected override string FormatItem(TItem item)
        {
            return _formatHandler(item);
        }
    }
}
