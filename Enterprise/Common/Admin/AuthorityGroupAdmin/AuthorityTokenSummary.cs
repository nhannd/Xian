#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;


namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	[DataContract]
	public class AuthorityTokenSummary : DataContractBase
	{
		public AuthorityTokenSummary(string name)
		{
			Name = name;
			FormerIdentities = new List<string>();
		}

		public AuthorityTokenSummary(string name, string description)
		{
			Name = name;
			Description = description;
			FormerIdentities = new List<string>();
		}

		public AuthorityTokenSummary(string name, string definingAssembly, string description, IEnumerable<string> formerIdentities)
		{
			Name = name;
			DefiningAssembly = definingAssembly;
			Description = description;
			FormerIdentities = new List<string>(formerIdentities);
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string DefiningAssembly;

		[DataMember]
		public string Description;

		[DataMember]
		public List<string> FormerIdentities;
	}
}
