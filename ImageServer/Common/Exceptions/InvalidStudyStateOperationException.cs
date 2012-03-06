#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents an exception thrown when the study state is invalid for the operation.
    /// </summary>
    public class InvalidStudyStateOperationException : System.Exception
    {
        public InvalidStudyStateOperationException(string message):base(message)
        {
            
        }
    }
}
