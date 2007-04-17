using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class UpdateNoteCategoryResponse : DataContractBase
    {
        public UpdateNoteCategoryResponse(NoteCategorySummary summary)
        {
            this.NoteCategory = summary;
        }

        [DataMember]
        public NoteCategorySummary NoteCategory;
    }
}
