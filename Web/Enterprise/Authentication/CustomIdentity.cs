#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    /// <summary>
    /// Custom Identity
    /// </summary>
    public class CustomIdentity : GenericIdentity
    {
        private readonly string _displayName;

        public CustomIdentity(string loginId, string displayName)
            : base(loginId)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// Formatted name of the identity
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
        }
    }
}