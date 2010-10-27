#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Request object for the <see cref="IUsageTracking.Register"/>
    /// </summary>
    [DataContract]
    public class RegisterResponse : IExtensibleDataObject
    {
        #region IExtensibleDataObject Members

        /// <summary>
        /// Extensible data for serialization.
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion

        /// <summary>
        /// A message to be displayed on the client.
        /// </summary>
        [DataMember]
        public DisplayMessage Message { get; set; }       
    }
}
