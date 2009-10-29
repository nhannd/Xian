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
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{

    ///<summary>
    /// Default implementation of <see cref="IPagingController{TItem}"/>.
    ///</summary>
	///<typeparam name="TItem"></typeparam>
    public class PagingController<TItem> : IPagingController<TItem>
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="firstRow"></param>
		/// <param name="maxRows"></param>
		/// <returns></returns>
		public delegate IList<TItem> PageQueryDelegate(int firstRow, int maxRows);
		
		private static readonly int _defaultPageSize = 50;
        private int _pageSize;
        private int _currentPageNumber;
        private bool _hasNext;
        private readonly PageQueryDelegate _queryDelegate;

		private event EventHandler _pageChanged;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pageSize"></param>
		/// <param name="queryDelegate"></param>
        public PagingController(int pageSize, PageQueryDelegate queryDelegate)
        {
            _pageSize = pageSize;
            _queryDelegate = queryDelegate;
            _currentPageNumber = 0;
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="queryDelegate"></param>
        public PagingController(PageQueryDelegate queryDelegate)
            : this(_defaultPageSize, queryDelegate)
        {
        }

        #region IPagingController<T> Members

    	/// <summary>
    	/// Gets or sets the number of items per page.
    	/// </summary>
    	public int PageSize
        {
            get {return _pageSize;}
            set {_pageSize = value;}
        }

    	/// <summary>
    	/// Gets a value indicating whether there is a next page.
    	/// </summary>
    	/// <returns></returns>
    	public bool HasNext
        {
            get { return _hasNext; }
        }

    	/// <summary>
    	/// Gets a value indicating whether there is a previous page.
    	/// </summary>
    	/// <returns></returns>
    	public bool HasPrevious
        {
            get { return _currentPageNumber > 0; }
        }

    	/// <summary>
    	/// Gets the next page of items.
    	/// </summary>
    	/// <returns></returns>
    	public IList<TItem> GetNext()
        {
            var results = DoQuery(NextPage());
            _currentPageNumber++;
			EventsHelper.Fire(_pageChanged, this, EventArgs.Empty);
			return results;
        }

    	/// <summary>
    	/// Gets the previous page of items.
    	/// </summary>
    	/// <returns></returns>
    	public IList<TItem> GetPrevious()
        {
            var results = DoQuery(PrevPage());
            _currentPageNumber--;
			EventsHelper.Fire(_pageChanged, this, EventArgs.Empty);
			return results;
        }

    	/// <summary>
    	/// Resets this instance to the first page of items.
    	/// </summary>
    	/// <returns></returns>
    	public IList<TItem> GetFirst()
        {
            _currentPageNumber = 0;
            var results = DoQuery(FirstPage());
        	EventsHelper.Fire(_pageChanged, this, EventArgs.Empty);
            return results;
        }

    	/// <summary>
    	/// Occurs when the current page changes (by calling any of <see cref="GetFirst"/>, <see cref="GetNext"/> or <see cref="GetPrevious"/>.
    	/// </summary>
    	public event EventHandler PageChanged
		{
			add { _pageChanged += value; }
			remove { _pageChanged -= value; }
		}

        #endregion

        private IList<TItem> DoQuery(int firstRow)
        {
        	var results = _queryDelegate(firstRow, _pageSize + 1) ?? new List<TItem>();

            if (results.Count == _pageSize + 1)
            {
                _hasNext = true;
                results.RemoveAt(_pageSize);
            }
            else
            {
                _hasNext = false;
            }

            return results;
        }

        private int NextPage()
        {
        	return HasNext ? (_currentPageNumber + 1)*_pageSize : 0;
        }

    	private int PrevPage()
    	{
    		return HasPrevious ? (_currentPageNumber - 1)*_pageSize : 0;
    	}

    	private static int FirstPage()
        {
            return 0;
        }
    }
}