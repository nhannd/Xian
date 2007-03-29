using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class NoteCategoryDetail : DataContractBase, ICloneable
    {
        public NoteCategoryDetail(string name, string description, EnumValueInfo severity)
        {
            this.Name = name;
            this.Description = description;
            this.Severity = severity;
        }

        public NoteCategoryDetail()
        {
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

        #region ICloneable Members

        public object Clone()
        {
            NoteCategoryDetail clone = new NoteCategoryDetail();
            clone.Name = this.Name;
            clone.Description = this.Description;
            clone.Severity = (EnumValueInfo)this.Severity.Clone();
            return clone;
        }

        #endregion
    }
}
