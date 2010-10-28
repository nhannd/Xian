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
    /// <summary>
    /// Indicates that an entity's version has changed, and does not match the requested version
    /// </summary>
    public class EntityVersionException : PersistenceException
    {
        public EntityVersionException(object oid, Exception inner)
            : base(string.Format(SR.ExceptionEntityVersion, oid), inner)
        {
        }
    }
}
