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
