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

namespace ClearCanvas.Enterprise.Core
{
    public abstract class ValueObjectSearchCriteria : SearchCriteria
    {
        public ValueObjectSearchCriteria(string key)
            :base(key)
        {
        }

        public ValueObjectSearchCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ValueObjectSearchCriteria(ValueObjectSearchCriteria other)
            :base(other)
        {
        }
   }
}
