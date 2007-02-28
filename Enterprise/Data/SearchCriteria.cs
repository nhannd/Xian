using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Data
{
    /// <summary>
    /// Abstract base class for all search criteria classes.
    /// 
    /// 
    /// </summary>
    public abstract class SearchCriteria
    {
        private string _key;
        private Dictionary<string, SearchCriteria> _subCriteria;

        public SearchCriteria(string key)
        {
            _key = key;
            _subCriteria = new Dictionary<string, SearchCriteria>();
        }

        public SearchCriteria()
            :this(null)
        {
        }

        /// <summary>
        /// Returns the key (e.g the property) on which this criteria is defined.  This is intentionally
        /// implemented as a method, rather than a property, so that there is no chance that a sub-class
        /// property will conflict.
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            return _key;
        }

        public IDictionary<string, SearchCriteria> SubCriteria
        {
            get { return _subCriteria; }
        }

        public virtual bool IsEmpty
        {
            get
            {
                foreach (SearchCriteria criteria in _subCriteria.Values)
                {
                    if (!criteria.IsEmpty)
                        return false;
                }
                return true;
            }
        }
    }
}
