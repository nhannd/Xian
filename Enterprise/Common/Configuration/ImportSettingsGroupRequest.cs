#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
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
