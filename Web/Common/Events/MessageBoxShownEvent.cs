#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Web.Common.Entities;

namespace ClearCanvas.Web.Common.Events
{
    [DataContract(Namespace = Namespace.Value)]
    public class MessageBoxShownEvent : Event
    {
        [DataMember(IsRequired = true)]
        public MessageBox MessageBox { get; set; }

        public override string ToString()
        {
            return string.Format("MessageBoxShownEvent [{0}]", MessageBox);
        }
    }
}
