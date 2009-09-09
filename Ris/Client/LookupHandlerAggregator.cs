using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public abstract class LookupHandlerAggregator : ILookupHandler
	{
		#region SuggestionProviderAggregator class

		class SuggestionProviderAggregator : ISuggestionProvider
		{
			private readonly LookupHandlerAggregator _owner;
			private event EventHandler<SuggestionsProvidedEventArgs> _suggestionsProvided;

			public SuggestionProviderAggregator(LookupHandlerAggregator owner)
			{
				_owner = owner;
				foreach (var handler in _owner.ChildHandlers)
				{
					handler.SuggestionProvider.SuggestionsProvided += SuggestionsProvidedEventHandler;
				}
			}

			public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided
			{
				add { _suggestionsProvided += value; }
				remove { _suggestionsProvided -= value; }
			}

			public void SetQuery(string query)
			{
				foreach (var handler in _owner.ChildHandlers)
				{
					handler.SuggestionProvider.SetQuery(query);
				}
			}

			private void SuggestionsProvidedEventHandler(object sender, SuggestionsProvidedEventArgs e)
			{
				var child = (ILookupHandler)sender;
				var items = _owner.SuggestedItems[child];
				items.Clear();
				items.AddRange(new TypeSafeEnumerableWrapper<object>(e.Items));

				// provide aggregate list of items
				var aggregate = CollectionUtils.Concat(new List<List<object>>(_owner.SuggestedItems.Values));
				EventsHelper.Fire(_suggestionsProvided, this, new SuggestionsProvidedEventArgs(aggregate));
			}
		}

		#endregion

		private readonly ISuggestionProvider _suggestionProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="childHandlers"></param>
		protected LookupHandlerAggregator(ILookupHandler[] childHandlers)
		{
			this.ChildHandlers = childHandlers;

			// initialize the dictionary with an empty list for each handler
			this.SuggestedItems = new Dictionary<ILookupHandler, List<object>>();
			foreach (var handler in childHandlers)
			{
				this.SuggestedItems.Add(handler, new List<object>());
			}
			_suggestionProvider = new SuggestionProviderAggregator(this);
		}

		/// <summary>
		/// Attempts to resolve a query to a single item, optionally interacting with the user.
		/// </summary>
		/// <remarks>
		/// </remarks>
		/// <param name="query">The text query.</param>
		/// <param name="interactive">True if interaction with the user is allowed.</param>
		/// <param name="result">The singular result.</param>
		/// <returns>True if the query was resolved to a singular item, otherwise false.</returns>
		public bool Resolve(string query, bool interactive, out object result)
		{
			if(interactive)
			{
				ResolveNameInteractive(query, out result);	
			}

			// otherwise give each child a chance to resolve it without interaction
			var results = new List<object>();
			foreach (var handler in ChildHandlers)
			{
				object temp;
				if (handler.Resolve(query, false, out temp))
					results.Add(temp);
			}

			// check whether we've resolved it to a singular value
			result = results.Count == 1 ? CollectionUtils.FirstElement(results) : null;
			return result != null;
		}

		/// <summary>
		/// Formats an item for display in the user-interface.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public string FormatItem(object item)
		{
			// determine which handler should format the item
			foreach (var kvp in SuggestedItems)
			{
				if (kvp.Value.Contains(item))
					return kvp.Key.FormatItem(item);
			}
			return null;
		}

		/// <summary>
		/// Gets a suggestion provider that provides suggestions for the lookup field.
		/// </summary>
		public ISuggestionProvider SuggestionProvider
		{
			get { return _suggestionProvider; }
		}

		/// <summary>
		/// Attempts to resolve the specified query to a single result, with user input.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		protected abstract bool ResolveNameInteractive(string query, out object result);

		private ILookupHandler[] ChildHandlers { get; set; }
		private Dictionary<ILookupHandler, List<object>> SuggestedItems { get; set; }

	}
}
