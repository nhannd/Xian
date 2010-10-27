#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin
{
    [DataContract]
	public class ListProcedureTypeGroupsRequest : ListRequestBase
    {
		public ListProcedureTypeGroupsRequest()
		{
		}

		public ListProcedureTypeGroupsRequest(SearchResultPage page)
			:base(page)
		{
		}

		public ListProcedureTypeGroupsRequest(EnumValueInfo categoryFilter, SearchResultPage page)
			:base(page)
		{
			this.CategoryFilter = categoryFilter;
		}

		[DataMember]
		public EnumValueInfo CategoryFilter;
	}
}