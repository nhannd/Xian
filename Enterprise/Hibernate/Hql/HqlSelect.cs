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
    public class HqlSelect : HqlElement
    {
        private string _variable;

        /// <summary>
        /// Constructs a select on the specified HQL variable
        /// </summary>
        /// <param name="variable">The HQL variable to select</param>
        public HqlSelect(string variable)
        {
            _variable = variable;
        }

        public override string Hql
        {
            get { return _variable; }
        }
    }
}
