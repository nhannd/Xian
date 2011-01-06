#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
	[DataContract]
	public class ListRequestBase : PagedDataContractBase
	{
		public ListRequestBase()
		{
		}


		public ListRequestBase(SearchResultPage page)
			: base(page)
		{
		}

		/// <summary>
		/// Specifies whether to include de-activated items in the results.  This value is ignored
		/// if the entity does not support the notion of de-activation.
		/// </summary>
		[DataMember]
		public bool IncludeDeactivated;
	}
}
