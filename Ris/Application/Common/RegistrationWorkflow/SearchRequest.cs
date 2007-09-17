using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class SearchRequest : DataContractBase
    {
        public SearchRequest(SearchData searchData)
        {
            this.SearchData = searchData;
        }

        [DataMember]
        public SearchData SearchData;
    }
}
