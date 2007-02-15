using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Common
{
    public delegate IList<U> PageSearchDelegate<U>(SearchCriteria criteria, SearchResultPage page);

    public class PagingController<T> : IPagingController<T>
    {
        private static readonly int _defaultPageSize = 50;
        private int _pageSize;
        private int _currentPageNumber;
        private bool _hasNext;
        private PageSearchDelegate<T> _searchDelegate;
        private SearchCriteria _criteria;
        public event QueryEventHandler OnInitialQueryEvent;

        #region IPagingController<T> Members

        public PagingController(int pageSize, PageSearchDelegate<T> searchDelegate)
        {
            _pageSize = pageSize;
            _searchDelegate = searchDelegate;
            _currentPageNumber = 0;
            _criteria = null;
        }

        public PagingController(PageSearchDelegate<T> searchDelegate)
            : this(_defaultPageSize, searchDelegate)
        {
        }

        public int PageSize
        {
            get {return _pageSize;}
            set {_pageSize = value;}
        }

        public bool HasNext
        {
            get { return _hasNext; }
        }

        public bool HasPrev
        {
            get { return _currentPageNumber > 0; }
        }

        public IList<T> GetNext()
        {
            IList<T> results = DoQuery(NextPage());
            _currentPageNumber++;
            return results;
        }

        public IList<T> GetPrev()
        {
            IList<T> results = DoQuery(PrevPage());
            _currentPageNumber--;
            return results;
        }

        public IList<T> GetFirst(SearchCriteria criteria)
        {
            _criteria = criteria;
            _currentPageNumber = 0;
            IList<T> results = DoQuery(FirstPage());
            OnInitialQueryEvent();
            return results;
        }

        #endregion

        private IList<T> DoQuery(SearchResultPage page)
        {
            IList<T> results;
            results = _searchDelegate(_criteria, page);

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

        private SearchResultPage NextPage()
        {
            if (HasNext)
            {
                return new SearchResultPage((_currentPageNumber + 1) * _pageSize, _pageSize + 1);
            }
            else
            {
                return null;
            }
        }

        private SearchResultPage PrevPage()
        {
            if (HasPrev)
            {
                return new SearchResultPage((_currentPageNumber - 1) * _pageSize, _pageSize + 1);
            }
            else
            {
                return null;
            }
        }

        private SearchResultPage FirstPage()
        {
            return new SearchResultPage(0, _pageSize + 1);
        }
    }
}