#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class LoadCannedTextForEditRequest : DataContractBase
	{
		public LoadCannedTextForEditRequest(EntityRef cannedTextRef)
		{
			this.CannedTextRef = cannedTextRef;
		}

		public LoadCannedTextForEditRequest(
			string name,
			string category,
			string staffId,
			string staffGroupName)
		{
			this.Name = name;
			this.Category = category;
			this.StaffId = staffId;
			this.StaffGroupName = staffGroupName;
		}

		[DataMember]
		public EntityRef CannedTextRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public string StaffId;

		[DataMember]
		public string StaffGroupName;
	}
}
