using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Threading;
using System.Diagnostics;

namespace ClearCanvas.Ris.Client
{
    public class RemoteSuggestionProvider<TItem> : ISuggestionProvider
    {
        #region State classes (state pattern implementation)

        abstract class State
        {
            protected readonly RemoteSuggestionProvider<TItem> _owner;
            public State(RemoteSuggestionProvider<TItem> owner)
            {
                _owner = owner;
            }

            public abstract void OnQueryTextUpdate(string text);
            public abstract void OnRequestCompleted(bool success, TItem[] results);
        }

        class AsyncDefaultState : State
        {
            public AsyncDefaultState(RemoteSuggestionProvider<TItem> owner)
                :base(owner)
            {
            }

            public override void OnQueryTextUpdate(string text)
            {
                if(_owner.IsRefinementOfLastRequestText(text))
                {
                    _owner.PostItemsMatching(text);
                }
                else
                {
                    // not a refinement of the previous query (or there was no previous query)
                    // try to begin a new request
                    if (_owner.BeginRequest(text))
                    {
                        // modify state
                        _owner._state = new RequestPendingState(_owner, text);
                    }
                }
            }

            public override void OnRequestCompleted(bool success, TItem[] results)
            {
                // should never get here in AsyncDefaultState
                throw new Exception("The method or operation is not implemented.");
            }
            
        }

        class RequestPendingState : State
        {
            private readonly string _query;

            public RequestPendingState(RemoteSuggestionProvider<TItem> owner, string query)
                :base(owner)
            {
                _query = query;
            }

            private void TryNewRequest(string text)
            {
                if (_owner.BeginRequest(text))
                {
                    _owner._state = new RequestPendingState(_owner, text);
                }
                else
                {
                    _owner._state = new AsyncDefaultState(_owner);
                }
            }

            public override void OnQueryTextUpdate(string text)
            {
                // do nothing while a request is pending
            }

            public override void OnRequestCompleted(bool success, TItem[] results)
            {
                if(success)
                {
                    // set the shortlist to the results of the request, and store the query used to obtain the results
                    _owner._shortList = new List<TItem>(results);
                    _owner._lastSuccessfulRequestQueryText = _query;

                    // also need to sort the shortlist for presentation in the UI
                    _owner._shortList.Sort(delegate(TItem x, TItem y) { return _owner.FormatItem(x).CompareTo(_owner.FormatItem(y)); });

                    // if current query text is equal to, or a refinement of, request query text,
                    // then consider the request successful and post the matching items
                    if(_owner.IsRefinementOfLastRequestText(_owner._currentQueryText))
                    {
                        _owner.PostItemsMatching(_owner._currentQueryText);
                        _owner._state = new AsyncDefaultState(_owner);
                    }
                    else
                    {
                        // the query text has changed, so we need to issue a new request
                        TryNewRequest(_owner._currentQueryText);
                    }
                }
                else
                {
                    // the request was not successful
                    // if the query text has changed we need to issue a new request immediately
                    if (_owner._currentQueryText != _query)
                    {
                        TryNewRequest(_owner._currentQueryText);
                    }
                    else
                    {
                        // otherwise just return to default state
                        _owner._state = new AsyncDefaultState(_owner);
                    }
                }
            }
        }

        #endregion


        public delegate bool RemoteQueryDelegate<T>(string query, out T[] results);
        public delegate string FormatDelegate<T>(T obj);

        private EventHandler<SuggestionsProvidedEventArgs> _itemsProvided;
        private readonly RemoteQueryDelegate<TItem> _remoteQueryCallback;
        private readonly FormatDelegate<TItem> _formatHandler;

        private List<TItem> _shortList;
        private string _currentQueryText;
        private string _lastSuccessfulRequestQueryText;
        private BackgroundTask _requestTask;
        private State _state;

        public RemoteSuggestionProvider(RemoteQueryDelegate<TItem> remoteQueryCallback, FormatDelegate<TItem> formatHandler)
        {
            _remoteQueryCallback = remoteQueryCallback;
            _formatHandler = formatHandler;
            _state = new AsyncDefaultState(this);
        }

        #region ISuggestionProvider Members

        public event EventHandler<SuggestionsProvidedEventArgs> SuggestionsProvided
        {
            add { _itemsProvided += value; }
            remove { _itemsProvided -= value; }
        }

        public void SetQuery(string query)
        {
            _state.OnQueryTextUpdate(query);

            _currentQueryText = query;
        }

        #endregion

        private bool BeginRequest(string text)
        {
            if (!IsUsefulQueryText(text))
                return false;

            _requestTask = new BackgroundTask(
                delegate (IBackgroundTaskContext context)
                    {
                        try
                        {
                            string queryText = (string)context.UserState;
                            TItem[] results;
                            bool success = _remoteQueryCallback(queryText, out results);
                            context.Complete(success, results);
                        }
                        catch (Exception e)
                        {
                            context.Error(e);
                        }
                    }, false, text);
            _requestTask.Terminated += _requestTask_Terminated;
            _requestTask.Run();

            return true;
        }

        private void _requestTask_Terminated(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            if(e.Reason == BackgroundTaskTerminatedReason.Exception)
            {
                // not much we can do about it, except log it and return an empty list
                Platform.Log(LogLevel.Error, e.Exception);
                _state.OnRequestCompleted(false, new TItem[]{});
            }
            else
            {
                bool success = (bool)e.Results[0];
                TItem[] results = (TItem[])e.Results[1];

                _state.OnRequestCompleted(success, results);
            }
        }

        private bool IsUsefulQueryText(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Trim().Length > 0;
        }

        private bool IsRefinementOfLastRequestText(string text)
        {
            return !string.IsNullOrEmpty(_lastSuccessfulRequestQueryText) && text.StartsWith(_lastSuccessfulRequestQueryText);
        }

        private void PostItemsMatching(string text)
        {
            List<TItem> items = RefineShortList(text);
            SuggestionsProvidedEventArgs args = new SuggestionsProvidedEventArgs(items);
            EventsHelper.Fire(_itemsProvided, this, args);
        }

        private List<TItem> RefineShortList(string text)
        {
            // return only items that contain the query text
            // TODO: this could be made more sophisticated if necessary
            return _shortList.FindAll(
                delegate(TItem item)
                {
                    string itemText = FormatItem(item);
                    return itemText.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
                });
        }

        private string FormatItem(object item)
        {
            return _formatHandler((TItem)item);
        }
    }
}
