using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common.Configuration
{
    [DataContract]
    public class ImportSettingsGroupRequest : DataContractBase
    {
        public ImportSettingsGroupRequest(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            this.Group = group;
            this.Properties = properties;
        }

        /// <summary>
        /// Settings group to update.  Required.
        /// </summary>
        [DataMember]
        public SettingsGroupDescriptor Group;

        /// <summary>
        /// Settings properties of the specified <see cref="Group"/>. Optional.  If null,
        /// the properties will not be updated.  If specified, all properties must be included,
        /// because any that are not will be deleted.
        /// </summary>
        [DataMember]
        public List<SettingsPropertyDescriptor> Properties;
    }
}
