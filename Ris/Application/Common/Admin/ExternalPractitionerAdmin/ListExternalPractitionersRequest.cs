using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    [DataContract]
    public class ListExternalPractitionersRequest : PagedDataContractBase
    {
        public ListExternalPractitionersRequest()
        {
        }

        public ListExternalPractitionersRequest(string surname, string givenname)
        {
            this.LastName = surname;
            this.FirstName = givenname;
        }

        public ListExternalPractitionersRequest(string surname, string givenname, PageRequestDetail page)
            : this(surname, givenname)
        {
            this.PageRequest = page;
        }

        [DataMember]
        public string FirstName;

        [DataMember]
        public string LastName;
    }
}