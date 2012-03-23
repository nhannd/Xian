#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.DicomServer
{
    public static class DicomServerNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/dicomServer";
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationResult
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetDicomServerConfigurationRequest
    {}

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationResult
    {
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class UpdateDicomServerConfigurationRequest
    {
        [DataMember(IsRequired = true)]
        public DicomServerConfiguration Configuration { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomServerConfiguration
    {
        [DataMember(IsRequired = true)]
        public string AETitle { get; set; }

        [DataMember(IsRequired = true)]
        public int Port { get; set; }

        // TODO (Marmot): Gonzo?
        [DataMember(IsRequired = true)]
        public string HostName { get; set; }

        //TODO (Marmot) will this actually be how we store the file store location? Probably.
        [DataMember(IsRequired = true)]
        public string FileStoreLocation { get; set; }
    }
}
