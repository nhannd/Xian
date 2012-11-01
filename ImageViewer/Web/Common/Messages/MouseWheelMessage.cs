#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Messages
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MouseWheelMessage : Message
    {
        [DataMember(IsRequired = true)]
        public int Delta { get; set; }

        [DataMember(IsRequired = true)]
        public bool IsDiscardable { get; set; }

    }
}