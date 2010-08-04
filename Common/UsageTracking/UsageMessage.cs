#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Application specific Key/Value pair information for <see cref="UsageMessage"/>.
    /// </summary>
    [DataContract]
    public class UsageApplicationData : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// Key/Value pair for application specific usage data.
        /// </summary>
        [XmlAttribute("Key")]
        [DataMember(IsRequired = true)]
        public string Key { get; set; }

        /// <summary>
        /// Key/Value pair for application specific usage data.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Value { get; set; }

    }

    /// <summary>
    /// A product usage message for usage tracking.
    /// </summary>
    [DataContract]
    public class UsageMessage : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// The timestamp for the usage data.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The product being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Product { get; set; }

        /// <summary>
        /// The version of the product being tracked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Version { get; set; }

        /// <summary>
        /// The operating system version string the product is installed on.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string OS { get; set; }

        /// <summary>
        /// The region/culture information for the system that the product is installed on.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Region { get; set; }

        /// <summary>
        /// License information configured for the product.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string License { get; set; }

        /// <summary>
        /// A set of application data specific to the <see cref="Product"/>.
        /// </summary>
        [XmlArray("AppData")]
        [XmlArrayItem("UsageApplicationData")]
        [DefaultValue(null)]
        [DataMember(IsRequired = false)]
        public List<UsageApplicationData> AppData { get; set; }
    }
}
