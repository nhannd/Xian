using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{    
    [DataContract]
    public class WorkItemProgress : DataContractBase
    {
        [DataMember(IsRequired = false)]
        public string StatusDescription { get; set; }

        [DataMember(IsRequired = true)]
        public Decimal PercentComplete { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }
}
