#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    /// Thrown when an <see cref="EntityRef"/> cannot be resovled.
    /// </summary>
    public class EntityNotFoundException : PersistenceException
    {
        public EntityNotFoundException(Exception inner)
            : base(SR.ExceptionEntityNotFound, inner)
        {
        }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
