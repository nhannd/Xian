#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
