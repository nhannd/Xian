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

        [DataMember(IsRequired = true)]
        public string MimeType { get; set; }

        [DataMember(IsRequired = true)]
        public double JpegQuality { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsBase64 { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsDataZipped { get; set; }

        [DataMember(IsRequired = true)]
        public byte[] Data { get; set; }
    }
}
