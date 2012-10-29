using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common
{
    [DataContract(Namespace = Namespace.Value)]
    public class Image : DataContractBase
    {
        public static class MimeTypes
        {
            public const string Jpeg = "image/jpeg";
            public const string Png = "image/png";
            public const string Bmp = "image/bmp";
        }

        [DataMember(IsRequired = false)]
        public string MimeType { get; set; }

        [DataMember(IsRequired = false)]
        public double JpegQuality { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsDataZipped { get; set; }

        [DataMember(IsRequired = false)]
        public byte[] Data { get; set; }
    }
}
