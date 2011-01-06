#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Web.Common.Entities;
using ClearCanvas.Web.Common;

namespace ClearCanvas.ImageViewer.Web.Common.Events
{
    [DataContract(Namespace = ViewerNamespace.Value)]
    public class MessageBoxShownEvent : Event
    {
        [DataMember(IsRequired = true)]
        public MessageBox MessageBox { get; set; }
    }
}
