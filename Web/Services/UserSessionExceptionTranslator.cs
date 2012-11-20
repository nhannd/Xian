#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Web.Services
{
    [ExtensionOf(typeof(ExceptionTranslatorExtensionPoint))]
    internal class UserSessionExceptionTranslator : IExceptionTranslator
    {
        #region IExceptionTranslator Members

        public string Translate(Exception e)
        {
            if (e.GetType() == typeof(UserAccessDeniedException))
                return SR.MessageAccessDenied;
            if (e.GetType() == typeof(PasswordExpiredException))
                return SR.MessagePasswordExpired;
            if (e.GetType() == typeof(InvalidUserSessionException))
                return SR.MessageSessionEnded;
            if (e.GetType() == typeof(SessionDoesNotExistException))
                return SR.MessageSessionEnded;
            if (e.GetType() == typeof(SessionExpiredException))
                return SR.MessageSessionEnded;
            if (e.GetType() == typeof(RequestValidationException))
                return SR.MessageUnexpectedError;

            return default(string);
        }

        #endregion
    }
}