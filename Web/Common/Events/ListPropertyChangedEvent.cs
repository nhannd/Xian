using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common.Events
{
    [DataContract(Namespace = Namespace.Value)]
    public enum ListChangeType
    {
        [EnumMember]
        ItemAdded,
        [EnumMember]
        ItemRemoved,
        [EnumMember]
        ItemChanged,
        [EnumMember]
        ItemMoved
    }

    [DataContract(Namespace = Namespace.Value)]
    public class ListPropertyChangedEvent : Event
    {
        [DataMember(IsRequired = false)]
        public string ListPropertyName { get; set; }

        [DataMember(IsRequired = false)]
        public ListChangeType Change { get; set; }

        [DataMember(IsRequired = false)]
        public int Index { get; set; }

        [DataMember(IsRequired = false)]
        public object Value { get; set; }
    }
}
