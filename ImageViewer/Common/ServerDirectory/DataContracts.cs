using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class ServerDirectoryNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/serverDirectory";
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    [KnownType(typeof(DicomServerApplicationEntity))]
    [KnownType(typeof(StreamingServerApplicationEntity))]
    public class ServerDirectoryEntry : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    #region Get

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class GetServersRequest: DataContractBase
    {
        /// <summary>
        /// Specify an exact name to find one server.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        /// <summary>
        /// Specify an exact AE Title to find one or more servers.
        /// </summary>
        [DataMember(IsRequired = false)]
        public string AETitle { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class GetServersResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public List<ApplicationEntity> Servers { get; set; }
    }

    #endregion

    #region Update

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    #endregion

    #region Delete

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteServerResult : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteAllServersRequest : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteAllServersResult : DataContractBase
    {
    }

    #endregion

    #region Add

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class AddServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class AddServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }
    }

    #endregion

    #region Faults

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerNotFoundFault : DataContractBase
    {
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerExistsFault : DataContractBase
    {
    }

    #endregion
}
