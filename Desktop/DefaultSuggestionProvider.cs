#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A default implementation of <see cref="ISuggestionProvider"/>, dynamically providing suggested
	/// text based on user input.
	/// </summary>
    /// <remarks>
    /// This class assumes the full list of possible suggestions is available in memory.
    /// </remarks>
	/// <typeparam name="TItem">The type of object for which suggestions should be made.</typeparam>
    public class DefaultSuggestionProvider<TItem> : SuggestionProviderBase<TItem>
    {
        private IList<TItem> _sourceList;
        private Converter<TItem, string> _formatHandler;
        private Comparison<TItem> _sortComparison;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceList">The source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
        public DefaultSuggestionProvider(IList<TItem> sourceList, Converter<TItem, string> formatHandler)
            :this(sourceList, formatHandler, null)
        {
            
        }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="sourceList">The source list of objects.</param>
		/// <param name="formatHandler">A delegate that returns a formatted text string for the input object.</param>
		/// <param name="sortComparison">A <see cref="Comparison{T}"/> object used to sort the list of suggestions.</param>
		public DefaultSuggestionProvider(IList<TItem> sourceList, Converter<TItem, string> formatHandler, Comparison<TItem> sortComparison)
        {
            _sourceList = sourceList;
            _formatHandler = formatHandler;
            _sortComparison = sortComparison;
        }

        protected override List<TItem> GetShortList(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            else
            {
                // go back to original source
                List<TItem> sourceList = new List<TItem>(_sourceList);
                if (_sortComparison != null)
                    sourceList.Sort(_sortComparison);
                else
                    sourceList.Sort();
                return sourceList;
            }
        }

        protected override string FormatItem(TItem item)
        {
            return _formatHandler(item);
        }
    }
}
