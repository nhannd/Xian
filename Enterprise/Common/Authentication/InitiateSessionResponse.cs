#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Authentication
{
	[DataContract]
	public class InitiateSessionResponse : DataContractBase
	{
		public InitiateSessionResponse(SessionToken sessionToken, string[] authorityTokens, Guid[] authorityGroups, string displayName, string emailAddress)
		{
			this.SessionToken = sessionToken;
			this.AuthorityTokens = authorityTokens;
		    this.DisplayName = displayName;
		    this.DataGroupOids = authorityGroups;
		    this.EmailAddress = emailAddress;
		}

		/// <summary>
		/// Session token that identifies newly created session.
		/// </summary>
		[DataMember]
		public SessionToken SessionToken;

		/// <summary>
		/// User authority tokens, if requested.
		/// </summary>
		[DataMember]
		public string[] AuthorityTokens;

        /// <summary>
        /// Name of the user.
        /// </summary>
	    [DataMember] 
        public string DisplayName;

        /// <summary>
        /// Email Address of the user.
        /// </summary>
        [DataMember]
        public string EmailAddress;

        /// <summary>
        /// User data authority group oids, if requested.
        /// </summary>
        [DataMember]
        public Guid[] DataGroupOids;
    }
}
