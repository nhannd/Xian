using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Entities
{
    [JavascriptModule("ClearCanvas/Controllers/ImageViewer/MagnificationController")]
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class Magnifier : Entity
    {
        [DataMember(IsRequired = true)]
        public Size Size { get; set; }

        [DataMember(IsRequired = true)]
        public Image Image { get; set; }

        [DataMember(IsRequired = true)]
        public Position Location { get; set; }
    }
}
