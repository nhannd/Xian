using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolCodeDetail : DataContractBase
    {
        public ProtocolCodeDetail(string name, string description)
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