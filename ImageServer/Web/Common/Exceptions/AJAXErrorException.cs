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

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class AJAXErrorException : BaseWebException
    {
        public AJAXErrorException(string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.AJAXError);
            ErrorDescription = ExceptionMessages.AJAXErrorDescription;
            LogMessage = logMessage;
        }

        public AJAXErrorException()
        {
            ErrorMessage = string.Format(ExceptionMessages.AJAXError);
            ErrorDescription = ExceptionMessages.AJAXErrorDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
