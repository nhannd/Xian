using System.Runtime.Serialization;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Common.Entities;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
    [DataContract(Namespace = Namespace.Value)]
    public class WebLayoutChangerAction : WebAction
    {
        [DataMember(IsRequired = true)]
        public int MaxRows { get; set; }

        [DataMember(IsRequired = true)]
        public int MaxColumns { get; set; }

        [DataMember(IsRequired = true)]
        public string ActionID { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", base.ToString(), ActionID);
        }
    }
}