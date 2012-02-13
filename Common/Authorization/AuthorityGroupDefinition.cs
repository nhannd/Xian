#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
		/// <param name="dataGroup">Tells if the group is an authority group for controlling access to data.</param>
		/// <param name="description">The description of the authority group.</param>
        public AuthorityGroupDefinition(string name, string description, bool dataGroup, string[] tokens)
        {
            Name = name;
            Tokens = tokens;
		    Description = description;
            DataGroup = dataGroup;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the authority group.</param>
        /// <param name="tokens">The associated authority group tokens.</param>
        public AuthorityGroupDefinition(string name, string[] tokens)
        {
            Name = name;
            Tokens = tokens;
            Description = name;
            DataGroup = false;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Description
        {
            get; private set;
        }


        /// <summary>
        /// Gets a bool signaling if the authority group is for Data access.
        /// </summary>
        public bool DataGroup
        {
            get; private set;
        }


        /// <summary>
        /// Gets the set of tokens that are assigned to the group.
        /// </summary>
        public string[] Tokens
        {
            get; private set;
        }
    }
}
