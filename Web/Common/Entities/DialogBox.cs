#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common.Entities
{
    [JavascriptModule("ClearCanvas/Controllers/DialogBoxController", LoadAsynchronously = false)]
    [DataContract(Namespace = Namespace.Value)]
    public class DialogBox : Entity
    {
        [DataMember(IsRequired = true)]
        public string Title { get; set; }

        [DataMember(IsRequired = true)]
        public Entity ApplicationComponent { get; set; }
    }
}