using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class ListAllNoteCategoriesResponse : DataContractBase
    {
        public ListAllNoteCategoriesResponse(List<NoteCategorySummary> NoteCategories)
        {
            this.NoteCategories = NoteCategories;
        }

        [DataMember]
        public List<NoteCategorySummary> NoteCategories;
    }

}
