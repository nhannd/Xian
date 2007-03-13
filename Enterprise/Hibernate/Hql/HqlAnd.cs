using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// A subclass of <see cref="HqlCondition"/> that represents an AND operation across a set of Hql conditions
    /// </summary>
    public class HqlAnd : HqlJunction
    {
        /// <summary>
        /// Creates an empty <see cref="HqlAnd"/>
        /// </summary>
        public HqlAnd()
        {
        }

        /// <summary>
        /// Creates an <see cref="HqlAnd"/> initialized with the specified conditions
        /// </summary>
        /// <param name="conditions"></param>
        public HqlAnd(IEnumerable<HqlCondition> conditions)
            : base(conditions)
        {
        }

        protected override string Operator
        {
            get { return "and"; }
        }
    }
}
