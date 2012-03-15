#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextDetail : DataContractBase
	{
		public CannedTextDetail()
		{
		}

		public CannedTextDetail(string name, string category, StaffGroupSummary staffGroup, string text)
		{
			this.Name = name;
			this.Category = category;
			this.StaffGroup = staffGroup;
			this.Text = text;
		}

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string Text;

		public bool IsPersonal
		{
			get { return !this.IsGroup; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
