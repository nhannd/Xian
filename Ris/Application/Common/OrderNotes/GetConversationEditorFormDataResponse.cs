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

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
	[DataContract]
	public class GetConversationEditorFormDataResponse : DataContractBase
	{
		public GetConversationEditorFormDataResponse(List<StaffGroupSummary> onBehalfOfGroupChoices)
		{
			OnBehalfOfGroupChoices = onBehalfOfGroupChoices;
			RecipientStaffs = new List<StaffSummary>();
			RecipientStaffGroups = new List<StaffGroupSummary>();
		}

		/// <summary>
		/// The on-behalf-of group choices for the current user.
		/// </summary>
		[DataMember]
		public List<StaffGroupSummary> OnBehalfOfGroupChoices;

		/// <summary>
		/// Staff summaries for recipient staff, specified in the request.
		/// </summary>
		[DataMember]
		public List<StaffSummary> RecipientStaffs;

		/// <summary>
		/// Group summaries for recipient groups, specified in the request.
		/// </summary>
		[DataMember]
		public List<StaffGroupSummary> RecipientStaffGroups;
	}
}
