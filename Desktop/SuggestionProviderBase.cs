using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
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
		#region State classes (state pattern implementation)

		/// <summary>
		/// Base class for states
		/// </summary>
		abstract class State
		{
			protected readonly SuggestionProviderBase<TItem> _owner;
			public State(SuggestionProviderBase<TItem> owner)
			{
				_owner = owner;
			}


			public abstract void Begin();
			public abstract void Update(string query);

			protected void BeginRequest(string query)
			{
				_owner.ChangeState(new ShortlistRequestState(_owner, query));
			}
		}

		/// <summary>
		/// Defines the state when the shortlist is not known.
		/// </summary>
		class CleanState : State
		{
			public CleanState(SuggestionProviderBase<TItem> owner)
				: base(owner)
			{
			}

			public override void Begin()
			{
			}

			public override void Update(string query)
			{
				BeginRequest(query);
			}

		}

		/// <summary>
		/// Defines the state where a request for a shortlist is in progress.
		/// </summary>
		class ShortlistRequestState : State
		{
			private BackgroundTask _task;
			private readonly string _query;
			private string _currentQuery;

			public ShortlistRequestState(SuggestionProviderBase<TItem> owner, string query)
				: base(owner)
			{
				_currentQuery = _query = query;
			}

			public override void Begin()
			{
				_task = new BackgroundTask(
					delegate(IBackgroundTaskContext context)
					{
						try
						{
							IList<TItem> results = _owner.GetShortList(_query);
							context.Complete(results);
						}
						catch (Exception e)
						{
							context.Error(e);
						}
					}, false, _query);
				_task.Terminated += OnTerminated;
				_task.Run();
			}

			public override void Update(string query)
			{
				// do nothing while a request is pending
				_currentQuery = query;
			}

			private void OnTerminated(object sender, BackgroundTaskTerminatedEventArgs e)
			{
				if (e.Reason == BackgroundTaskTerminatedReason.Exception)
				{
					// not much we can do about it, except log it and return to blank state
					Platform.Log(LogLevel.Error, e.Exception);
					_owner.ChangeState(new CleanState(_owner));
				}
				else
				{
					IList<TItem> shortlist = (IList<TItem>)e.Result;
					if(shortlist == null)
					{
						// the request did not return a shortlist
						// has the query been updated in the interim? if so, begin a new request immediately
						if (_currentQuery != _query)
						{
							BeginRequest(_currentQuery);
						}
						else
						{
							// return to blank state
							_owner.ChangeState(new CleanState(_owner));
						}
					}
					else
					{
						// the request obtained a shortlist
						// has the query been updated in the interim? if so, is it a refinement of the query that obtained the shortlist?
						if (_currentQuery == _query || _owner.IsQueryRefinement(_currentQuery, _query))
						{
							_owner.ChangeState(new ShortlistKnownState(_owner, _query, shortlist));
						}
						else
						{
							// the query was updated and is not a refinement, hence the shortlist is not valid
							// begin a new request
							BeginRequest(_currentQuery);
						}
					}
				}
			}

		}

		/// <summary>
		/// Defines the state where the shortlist is known.
		/// </summary>
		class ShortlistKnownState : State
		{
			private readonly string _query;
			private readonly IList<TItem> _shortlist;

			public ShortlistKnownState(SuggestionProviderBase<TItem> owner, string query, IList<TItem> shortlist)
				: base(owner)
			{
				_query = query;
				_shortlist = shortlist;
			}

			public override void Begin()
			{
				Update(_query);
			}

			public override void Update(string query)
			{
				if(query == _query || _owner.IsQueryRefinement(query, _query))
				{
					IList<TItem> refinedList = _owner.RefineShortList(_shortlist, query);
					_owner.PostSuggestions(refinedList);
				}
				else
				{
					// shortlist no longer valid - clear suggestions
					_owner.PostSuggestions(new TItem[]{});

					// request new shortlist
					BeginRequest(query);
				}
			}
		}

		#endregion

    	private State _state;
		private event EventHandler<SuggestionsProvidedEventArgs> _suggestionsProvided;

		/// <summary>
		/// Constructor.
		/// </summary>
        protected SuggestionProviderBase()
        {
			_state = new CleanState(this);
        }

		#region ISuggestionProvider Members

		/// <summary>
		/// Notifies the user-interfaces that an updated list of suggestions is available.
		/// </summary>
		event EventHandler<SuggestionsProvidedEventArgs> ISuggestionProvider.SuggestionsProvided
		{
			add { _suggestionsProvided += value; }
			remove { _suggestionsProvided -= value; }
		}

		/// <summary>
		/// Called by the user-inteface to inform this object of changes in the user query text.
		/// </summary>
		void ISuggestionProvider.SetQuery(string query)
		{
			Update(StringUtilities.EmptyIfNull(query));
		}

		#endregion

		#region Protected API

		/// <summary>
        /// Called to obtain the initial source list for the specified query.  May return null if no items are available.
        /// </summary>
        /// <remarks>
        /// <para>
		/// This method is called to obtain an initial list of items for a given query.  The method should return 
		/// null if no items are available for the specified query (or if the query is too "broad" to return any suggestions).
		/// This method is called repeatedly each time the user updates the query, until a non-null result is returned.
		/// Once this method returns a non-null result, it is not called again as long as subsequent queries are increasingly
		/// "specific", as defined by the implementation of <see cref="IsQueryRefinement"/>.  The method <see cref="RefineShortList"/> is called instead.
		/// However, as soon as a query is encountered that is less specific than the query that generated the shortlist
		/// (e.g. the user presses backspace enough times), this method will be called again to generate a new source list.
		/// </para>
		/// <para>
		/// In order to keep the UI responsive, this method is invoked on a background thread, meaning that the implementation
		/// is free to make remote calls or perform other time consuming tasks to generate the list, without fear of locking up the UI.
		/// </para>
        /// </remarks>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract IList<TItem> GetShortList(string query);

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
        protected virtual IList<TItem> RefineShortList(IList<TItem> shortList, string query)
        {
            // refine the short-list
            return CollectionUtils.Select(shortList,
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
		/// <param name="query"></param>
        /// <param name="previousQuery"></param>
        /// <returns></returns>
        /// <remarks>
        /// The default implementation of this method returns true if query starts with previousQuery.
        /// </remarks>
        protected virtual bool IsQueryRefinement(string query, string previousQuery)
        {
            return query.StartsWith(previousQuery);
        }

		#endregion

		#region Helpers

		/// <summary>
		/// Updates the state machine.
		/// </summary>
		/// <param name="query"></param>
		private void Update(string query)
        {
			_state.Update(query);
        }

		/// <summary>
		/// Called by the <see cref="State"/> classes to change the state of this object.
		/// </summary>
		/// <param name="state"></param>
		private void ChangeState(State state)
		{
			_state = state;
			_state.Begin();
		}

		/// <summary>
		/// Posts the specified list of suggested items to the consumer of this provider.
		/// </summary>
		/// <param name="suggestions"></param>
		private void PostSuggestions(IList<TItem> suggestions)
		{
			EventsHelper.Fire(_suggestionsProvided, this, new SuggestionsProvidedEventArgs(new List<TItem>(suggestions)));
		}

		#endregion
	}
}
