using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class GetConversationEditorFormDataResponse : DataContractBase
    {
    	public GetConversationEditorFormDataResponse(List<StaffGroupSummary> onBehalfOfGroupChoices)
    	{
    		OnBehalfOfGroupChoices = onBehalfOfGroupChoices;
    	}

    	[DataMember]
    	public List<StaffGroupSummary> OnBehalfOfGroupChoices;

        [DataMember]
        public List<StaffSummary> RecipientStaffs;

        [DataMember]
        public List<StaffGroupSummary> RecipientStaffGroups;
    }
}
