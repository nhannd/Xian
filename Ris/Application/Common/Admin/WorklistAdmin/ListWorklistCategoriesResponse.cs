using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListWorklistCategoriesResponse : DataContractBase
    {
        public ListWorklistCategoriesResponse(List<string> categories)
        {
            Categories = categories;
        }

        [DataMember]
        public List<string> Categories;
    }
}
