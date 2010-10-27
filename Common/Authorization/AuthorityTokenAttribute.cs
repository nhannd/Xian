#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Authorization
{
    /// <summary>
    /// Attribute used to define authority group tokens on types.
    /// </summary>
	/// <seealso cref="AuthorityGroupDefinition"/>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class AuthorityTokenAttribute : Attribute
    {
        private string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
        public AuthorityTokenAttribute()
        {
        }

		/// <summary>
		/// The token description.
		/// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
