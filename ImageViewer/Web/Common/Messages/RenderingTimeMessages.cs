#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
    public class RoundTripRenderingTimeMessage : Message
    {
        [DataMember(IsRequired = true)]
        public Size ImageSize { get; set; }

        [DataMember(IsRequired = true)]
        public double ValueMilliseconds { get; set; }
    }

    [DataContract(Namespace = ViewerNamespace.Value)]
    public class StackRenderingTimesMessage : Message
    {
        [DataMember(IsRequired = true)]
        public Size ImageSize { get; set; }

        [DataMember(IsRequired = true)]
        public double[] ValuesMilliseconds { get; set; }
    }
}
