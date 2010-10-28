#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Authorization
{
    /// <summary>
    /// Helper class for providing authority group definitions to be imported at deployment time.
    /// </summary>
	/// <seealso cref="AuthorityTokenAttribute"/>
	public class AuthorityGroupDefinition
    {
        private string _name;
        private string[] _tokens;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
        public AuthorityGroupDefinition(string name, string[] tokens)
        {
            _name = name;
            _tokens = tokens;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the set of tokens that are assigned to the group.
        /// </summary>
        public string[] Tokens
        {
            get { return _tokens; }
        }
    }
}
