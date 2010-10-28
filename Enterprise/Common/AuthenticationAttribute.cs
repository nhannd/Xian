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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// When applied to a service contract interface, specifies whether that service requires
    /// authentication or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AuthenticationAttribute : Attribute
    {
        /// <summary>
        /// Tests a service contract to see if it requires authentication.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        public static bool IsAuthenticationRequired(Type serviceContract)
        {
            AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceContract);
            return authAttr == null ? true : authAttr.AuthenticationRequired;
        }

        private readonly bool _required;

        public AuthenticationAttribute(bool required)
        {
            _required = required;
        }

        /// <summary>
        /// Gets a value indicating whether authentication is required.
        /// </summary>
        public bool AuthenticationRequired
        {
            get { return _required; }
        }
    }
}