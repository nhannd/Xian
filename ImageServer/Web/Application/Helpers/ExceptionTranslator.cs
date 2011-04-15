#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Common;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    internal static class ExceptionTranslator
    {
        public static string Translate(Exception ex)
        {
            if (ex.GetType().Equals(typeof(AuthorityGroupIsNotEmptyException)))
            {
                AuthorityGroupIsNotEmptyException exception = ex as AuthorityGroupIsNotEmptyException;
                return exception.UserCount == 1
                           ? string.Format(ErrorMessages.ExceptionAuthorityGroupIsNotEmpty_OneUser, exception.GroupName)
                           : string.Format(ErrorMessages.ExceptionAuthorityGroupIsNotEmpty_MultipleUsers, exception.GroupName, exception.UserCount);
            }

            return ex.Message;
        }
    }
}
