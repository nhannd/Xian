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
    public class RestartListenerRequest
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class RestartListenerResult
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetListenerStateRequest
    { }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class GetListenerStateResult
    {
        [DataMember(IsRequired = true)]
        public ServiceStateEnum State { get; set; }
    }

    [DataContract(Namespace = DicomServerNamespace.Value)]
    public class DicomServerConfiguration : IEquatable<DicomServerConfiguration>
    {
        [DataMember(IsRequired = true)]
        public string AETitle { get; set; }

        [DataMember(IsRequired = true)]
        public int Port { get; set; }

        [DataMember(IsRequired = true)]
        public string HostName { get; set; }

        public override int GetHashCode()
        {
            int hash = 0x5467912;

            if (AETitle != null)
                hash ^= AETitle.GetHashCode();
            if (HostName != null)
                hash ^= HostName.GetHashCode();

            hash ^= Port.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is DicomServerConfiguration)
                return Equals((DicomServerConfiguration) obj);

            return false;
        }

        //TODO (Marmot):Get rid of this.
        #region IEquatable<DicomServerConfiguration> Members

        public bool Equals(DicomServerConfiguration other)
        {
            return AETitle == other.AETitle &&
                   HostName == other.HostName &&
                   Port == other.Port;
        }

        #endregion
    }
}
