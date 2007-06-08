using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    public delegate IList<U> PageSearchDelegate<U>(int firstRow, int maxRows);

    public class PagingController<T> : IPagingController<T>
    {
        private static readonly int _defaultPageSize = 50;
        private int _pageSize;
        private int _currentPageNumber;
        private bool _hasNext;
        private readonly PageSearchDelegate<T> _searchDelegate;
        public event QueryEventHandler OnInitialQueryEvent;

        public PagingController(int pageSize, PageSearchDelegate<T> searchDelegate)
        {
            _pageSize = pageSize;
            _searchDelegate = searchDelegate;
            _currentPageNumber = 0;
        }

        public PagingController(PageSearchDelegate<T> searchDelegate)
            : this(_defaultPageSize, searchDelegate)
        {
        }

        #region IPagingController<T> Members

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

        public IList<T> GetFirst()
        {
            _currentPageNumber = 0;
            IList<T> results = DoQuery(FirstPage());
            OnInitialQueryEvent();
            return results;
        }

        #endregion

        private IList<T> DoQuery(int firstRow)
        {
            IList<T> results;
            results = _searchDelegate(firstRow, _pageSize + 1) ?? new List<T>();

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
            if (HasNext)
            {
                return (_currentPageNumber + 1) * _pageSize;
            }
            else
            {
                return 0;
            }
        }

        private int PrevPage()
        {
            if (HasPrev)
            {
                return (_currentPageNumber - 1) * _pageSize;
            }
            else
            {
                return 0;
            }
        }

        private static int FirstPage()
        {
            return 0;
        }
    }
}