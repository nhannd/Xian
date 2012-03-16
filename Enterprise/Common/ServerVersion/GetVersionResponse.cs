#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Enterprise.Common.ServerVersion
{
    [DataContract]
    public class GetVersionResponse : DataContractBase
    {
        public GetVersionResponse()
        {}

        [DataMember]
        public string Component;

        [DataMember]
        public string Edition;
        [DataMember]
        public int VersionMajor;

        [DataMember]
        public int VersionMinor;

        [DataMember]
        public int VersionBuild;

        [DataMember]
        public int VersionRevision;
    }
}