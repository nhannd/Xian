using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class NoteCategoryDetail : DataContractBase, ICloneable
    {
        public NoteCategoryDetail(string category, string description, EnumValueInfo severity)
        {
            this.Category = category;
            this.Description = description;
            this.Severity = severity;
        }

        public NoteCategoryDetail()
        {
        }

        [DataMember]
        public string Category;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

        #region ICloneable Members

        public object Clone()
        {
            NoteCategoryDetail clone = new NoteCategoryDetail();
            clone.Category = this.Category;
            clone.Description = this.Description;
            clone.Severity = (EnumValueInfo)this.Severity.Clone();
            return clone;
        }

        #endregion
    }
}
