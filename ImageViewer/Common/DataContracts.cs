#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common
{
    public static class ImageViewerNamespace
    {
        public const string Value = "http://www.clearcanvas.ca/imageviewer";
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    public enum ServiceStateEnum
    {
        [EnumMember]
        Stopped,
        [EnumMember]
        Starting,
        [EnumMember]
        Started,
        [EnumMember]
        Stopping
    }

    [DataContract(Namespace = ImageViewerNamespace.Value)]
    public class ServiceStateFault
    {
        [DataMember(IsRequired = true)]
        public ServiceStateEnum CurrentState { get; set; }
        
        [DataMember(IsRequired = true)]
        public ServiceStateEnum RequiredState { get; set; }
    }
}
