using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class GetConversationEditorFormDataRequest : DataContractBase
    {
        [DataMember]
        public List<string> RecipientStaffIDs;

        [DataMember]
        public List<string> RecipientStaffGroupNames;
    }
}
