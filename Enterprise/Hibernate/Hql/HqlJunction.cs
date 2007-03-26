using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
    /// <summary>
    /// A subclass of <see cref="HqlCondition"/> that models a junction of other Hql conditions.  See subclasses
    /// <see cref="HqlAnd"/> and <see cref="HqlOr"/>.
    /// </summary>
    public abstract class HqlJunction : HqlCondition
    {
        private List<HqlCondition> _conditions = new List<HqlCondition>();

        public HqlJunction()
            :base(null, null)
        {

        }

        public HqlJunction(IEnumerable<HqlCondition> conditions)
            :base(null, null)
        {
            _conditions.AddRange(conditions);
        }

        /// <summary>
        /// Gets the junction operator
        /// </summary>
        protected abstract string Operator { get; }

        /// <summary>
        /// Gets the set of conditions that are the operands of this junction
        /// </summary>
        public List<HqlCondition> Conditions
        {
            get { return _conditions; }
        }

        public override string Hql
        {
            get
            {
                if (_conditions.Count == 0)
                    throw new HqlException("Invalid HQL junction - no sub-conditions");

                // build the hql
                StringBuilder hql = new StringBuilder();
                if (_conditions.Count > 1)
                    hql.Append("(");

                hql.Append(_conditions[0].Hql);
                for (int i = 1; i < _conditions.Count; i++)
                {
                    hql.Append(" ").Append(this.Operator).Append(" ").Append(_conditions[i].Hql);
                }

                if (_conditions.Count > 1)
                    hql.Append(")");

                return hql.ToString();
            }
        }

        public override object[] Parameters
        {
            get
            {
                List<object> parameters = new List<object>();
                foreach (HqlCondition c in _conditions)
                {
                    parameters.AddRange(c.Parameters);
                }
                return parameters.ToArray();
            }
        }
    }
}
