#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// ProtocolStatus enumeration
    /// </summary>
    [EnumValueClass(typeof(ProtocolStatusEnum))]
    public enum ProtocolStatus
    {
        /// <summary>
        /// Pending
        /// </summary>
        [EnumValue("Pending", Description = "Protocol is pending")]
        PN,

        /// <summary>
        /// Protocolled
        /// </summary>
        [EnumValue("Protocolled", Description = "Protocol assigned and order accepted")]
        PR,

        /// <summary>
        /// Rejected
        /// </summary>
        [EnumValue("Rejected", Description = "Protocol assigned and order rejected")]
        RJ,

        /// <summary>
        /// Awaiting Approval
        /// </summary>
        [EnumValue("Awaiting Approval", Description = "Protocol submitted for approval by resident")]
        AA,

        /// <summary>
        /// Cancelled
        /// </summary>
        [EnumValue("Cancelled", Description = "Protocol is cancelled")]
        X
    }
}