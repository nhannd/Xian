#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class GetConversationEditorFormDataRequest : DataContractBase
    {
    	public GetConversationEditorFormDataRequest(List<string> recipientStaffIDs, List<string> recipientStaffGroupNames)
    	{
    		RecipientStaffIDs = recipientStaffIDs;
    		RecipientStaffGroupNames = recipientStaffGroupNames;
    	}

		/// <summary>
		/// List of staff IDs for which to staff summary information should be obtained.
		/// </summary>
    	[DataMember]
        public List<string> RecipientStaffIDs;

		/// <summary>
		/// List of groups for which to summary information should be obtained.
		/// </summary>
		[DataMember]
        public List<string> RecipientStaffGroupNames;
    }
}
