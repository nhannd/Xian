#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Request object for the <see cref="IUsageTracking.Register"/> service.
    /// </summary>
    [DataContract]
    public class RegisterRequest : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        [DefaultValue(null)]
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// The usage data to register.
        /// </summary>
        [DataMember]
        public UsageMessage Message { get; set; }
    }
}
