using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class AddProtocolCodeRequest : DataContractBase
    {
        public AddProtocolCodeRequest(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;
    }
}