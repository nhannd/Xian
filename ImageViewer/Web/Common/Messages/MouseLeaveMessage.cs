#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Messages
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MouseLeaveMessage : Message
    {
        public override string ToString()
        {
            return String.Format("Mouse Leave {0}", Identifier);
        }
    }
}
