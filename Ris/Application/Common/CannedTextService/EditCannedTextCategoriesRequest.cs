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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class EditCannedTextCategoriesRequest : DataContractBase
	{
		public EditCannedTextCategoriesRequest(List<EntityRef> cannedTextRefs, string category)
		{
			this.CannedTextRefs = cannedTextRefs;
			this.Category = category;
		}

		[DataMember]
		public List<EntityRef> CannedTextRefs;

		[DataMember]
		public string Category;
	}
}