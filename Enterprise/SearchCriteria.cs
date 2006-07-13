using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all search criteria classes.
    /// 
    /// Typically, a concrete search criteria class is created for any entity that is intended to be
    /// searchable.  To create such a class, extend this class and add member fields of type <see cref="SearchCondition"/>
    /// that have the same names as the corresponding properties of the searchable entity.
    /// 
    /// For example, for an entity Person defined as
    /// <code>
    /// class Person : Entity {
    ///     public string FirstName { get {...} }
    ///     public string LastName { get {...} }
    /// }
    /// </code>
    /// 
    /// the search criteria would be defined as 
    /// <code>
    /// class PersonSearchCriteria : SearchCriteria
    /// {
    ///     public SearchCondition{string} FirstName = new SearchCondition{string}();
    ///     public SearchCondition{string} LastName = new SearchCondition{string}();
    /// }
    /// </code>
    /// 
    /// Note that <see cref="SearchCondition"/> members need only be added for fields that are searchable.  It is
    /// important that they are named identically to the properties in the corresponding entity class.
    /// 
    /// </summary>
    public abstract class SearchCriteria
    {
        private SearchResultLimit _limit;

        public SearchCriteria()
        {
            _limit = new SearchResultLimit();
        }

        /// <summary>
        /// Used in paging scenarios to specify a limit clause
        /// </summary>
        public SearchResultLimit Limit
        {
            get { return _limit; }
        }
    }
}
