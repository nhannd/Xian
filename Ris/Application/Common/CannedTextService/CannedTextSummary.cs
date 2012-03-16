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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextSummary : DataContractBase
	{
		/// <summary>
		/// Define the maximum length of the TextSnippet
		/// </summary>
		public const int MaxTextLength = 128;

		public CannedTextSummary(EntityRef cannedTextRef
			, string name
			, string category
			, StaffSummary staff
			, StaffGroupSummary staffGroup
			, string textSnippet)
		{
			this.CannedTextRef = cannedTextRef;
			this.Name = name;
			this.Category = category;
			this.Staff = staff;
			this.StaffGroup = staffGroup;
			this.TextSnippet = textSnippet.Substring(0, textSnippet.Length < MaxTextLength ? textSnippet.Length : MaxTextLength);
		}

		[DataMember]
		public EntityRef CannedTextRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffSummary Staff;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string TextSnippet;

		public bool IsPersonal
		{
			get { return this.Staff != null; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
