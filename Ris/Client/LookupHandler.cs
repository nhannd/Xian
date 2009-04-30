#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// A RIS-specific abstract base implemenation of <see cref="ILookupHandler"/>.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TSummary"></typeparam>
    public abstract class LookupHandler<TRequest, TSummary> : ILookupHandler
        where TRequest : TextQueryRequest, new()
        where TSummary : DataContractBase

    {
        /// <summary>
        /// Minimum length the query string must be in order to initiate a query.
        /// </summary>
        private readonly int _minQueryStringLength = 2;

        /// <summary>
        /// Default maximum number of matches the query can return.
        /// </summary>
        private readonly int _defaultSpecificityThreshold = 100;

        private ISuggestionProvider _suggestionProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
        public LookupHandler()
        {
        }

		#region Public members

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="minQueryStringLength"></param>
		/// <param name="defaultSpecificityThreshold"></param>
		public LookupHandler(int minQueryStringLength, int defaultSpecificityThreshold)
		{
			Platform.CheckArgumentRange(minQueryStringLength, 1, int.MaxValue, "minQueryStringLength");
			Platform.CheckArgumentRange(defaultSpecificityThreshold, 1, int.MaxValue, "defaulSpecificityThreshold");

			_minQueryStringLength = minQueryStringLength;
			_defaultSpecificityThreshold = defaultSpecificityThreshold;
		}

		/// <summary>
		/// Attempts to resolve the specified query to a single result, returning true if successful.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public bool ResolveName(string query, out TSummary result)
		{
			return ResolveNameHelper(query, out result, new object[] { });
		}

		/// <summary>
		/// Attempts to resolve the specified query to multiple results, where the number of matches is less
		/// than the supplied <paramref name="specificityThreshold"/>, returning true if successful.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="specificityThreshold"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		public bool ResolveName(string query, int specificityThreshold, out TSummary[] results)
		{
			return ResolveNameHelper(query, specificityThreshold, out results);
		}

		/// <summary>
		/// Attempts to resolve the specified query to multiple results, where the number of matches is less
		/// than the default specificity threshold, returning true if successful.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="results"></param>
		/// <returns></returns>
		public bool ResolveName(string query, out TSummary[] results)
		{
			return ResolveNameHelper(query, _defaultSpecificityThreshold, out results);
		}

		/// <summary>
		/// Attempts to resolve the specified query to a single result, with user input.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public abstract bool ResolveNameInteractive(string query, out TSummary result);

		/// <summary>
		/// Formats the specified item for display.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract string FormatItem(TSummary item);

		#endregion

        #region ILookupHandler Members

    	/// <summary>
    	/// Gets a suggestion provider that provides suggestions for the lookup field.
    	/// </summary>
    	ISuggestionProvider ILookupHandler.SuggestionProvider
        {
            get
            {
                if (_suggestionProvider == null)
                {
                	_suggestionProvider = new DefaultSuggestionProvider<TSummary>(
                		delegate(string query)
                		{
                			TSummary[] results;
							return (StringUtilities.EmptyIfNull(query).Trim().Length >= _minQueryStringLength
								&& ResolveName(query, out results)) ? results : null;
                		},
                		FormatItem);
                }
                return _suggestionProvider;
            }
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
    	bool ILookupHandler.Resolve(string query, bool interactive, out object result)
        {
            TSummary r;
            bool success = interactive ? ResolveNameInteractive(query, out r) : ResolveName(query, out r);
            result = success ? r : null;
            return success;
        }

    	/// <summary>
    	/// Formats an item for display in the user-interface.
    	/// </summary>
    	/// <param name="item"></param>
    	/// <returns></returns>
    	string ILookupHandler.FormatItem(object item)
        {
            return FormatItem((TSummary)item);
        }

        #endregion

		#region Protected overridables

		protected abstract TextQueryResponse<TSummary> DoQuery(TRequest request);

		#endregion

		#region Protected Helpers

		protected bool ResolveNameHelper(string query, out TSummary result, params object[] additionalArgs)
        {
            TSummary[] results;
            bool success = ResolveNameHelper(query, 1, out results);
            result = success && results.Length > 0 ? results[0] : null;
            return success;
        }

        protected bool ResolveNameHelper(string query, int specificityThreshold, out TSummary[] results)
        {
            if (!string.IsNullOrEmpty(query) && query.Trim().Length >= _minQueryStringLength)
            {
                TRequest request = new TRequest();
                request.TextQuery = query;
                request.SpecificityThreshold = specificityThreshold;

                TextQueryResponse<TSummary> response = DoQuery(request);
                if (!response.TooManyMatches)
                {
                    results = new TSummary[response.Matches.Count];
                    response.Matches.CopyTo(results);
                    return true;
                }
            }

            // too many matches
            results = new TSummary[]{};
            return false;
        }

        #endregion

    }
}
