using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation
{
    [DataContract]
    public class DocumentationPageDetail : IEquatable<DocumentationPageDetail>
    {
        public DocumentationPageDetail(string url)
        {
            this.Url = url;
        }

        [DataMember]
        public string Url;

        #region IEquatable

        public bool Equals(DocumentationPageDetail documentationPageDetail)
        {
            if (documentationPageDetail == null) return false;
            return Equals(Url, documentationPageDetail.Url);
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return Equals(obj as DocumentationPageDetail);
        }

        public override int GetHashCode()
        {
            return Url != null ? Url.GetHashCode() : 0;
        }

        #endregion
    }
}