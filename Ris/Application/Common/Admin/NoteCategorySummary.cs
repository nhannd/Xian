using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class NoteCategorySummary : DataContractBase, ICloneable
    {
        public NoteCategorySummary(EntityRef noteCategoryRef, string name, string description, EnumValueInfo severity)
        {
            this.NoteCategoryRef = noteCategoryRef;
            this.Name = name;
            this.Description = description;
            this.Severity = severity;
        }

        public NoteCategorySummary()
        {
        }

        [DataMember]
        public EntityRef NoteCategoryRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

        #region ICloneable Members

        public object Clone()
        {
            NoteCategorySummary clone = new NoteCategorySummary();
            clone.NoteCategoryRef = this.NoteCategoryRef;
            clone.Name = this.Name;
            clone.Description = this.Description;
            clone.Severity = (EnumValueInfo)this.Severity.Clone();
            return clone;
        }

        #endregion
    }
}
