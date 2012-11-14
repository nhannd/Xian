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
    [JavascriptModule("ClearCanvas/Controllers/ProgressComponentController")]
    [DataContract(Namespace = Namespace.Value)]
    public class ProgressComponent : Entity
    {
        [DataMember(IsRequired = true)]
        public bool IsMarquee { get; set; }

        [DataMember(IsRequired = true)]
        public string Message { get; set; }

        [DataMember(IsRequired = true)]
        public string ButtonText { get; set; }

        [DataMember(IsRequired = true)]
        public int ProgressPercent { get; set; }

        [DataMember(IsRequired = true)]
        public bool CancelButtonVisible { get; set; }

        [DataMember(IsRequired = true)]
        public bool Cancelled { get; set; }
    }
}
