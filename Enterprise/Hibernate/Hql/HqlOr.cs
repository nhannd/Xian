using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// A subclass of <see cref="HqlCondition"/> that represents an OR operation across a set of Hql conditions
    /// </summary>
    public class HqlOr : HqlJunction
    {
        /// <summary>
        /// Creates an empty <see cref="HqlOr"/>
        /// </summary>
        public HqlOr()
        {
        }

        /// <summary>
        /// Creates an <see cref="HqlOr"/> initialized with the specified conditions
        /// </summary>
        /// <param name="conditions"></param>
        public HqlOr(IEnumerable<HqlCondition> conditions)
            : base(conditions)
        {
        }

        protected override string Operator
        {
            get { return "or"; }
        }
    }
}
