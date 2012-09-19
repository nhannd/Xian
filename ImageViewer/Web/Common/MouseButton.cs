#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Web.Common
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public enum MouseButton
    {
        [EnumMember]
        None,
        [EnumMember]
        Left,
        [EnumMember]
        Right,
        [EnumMember]
        Middle
    }
}