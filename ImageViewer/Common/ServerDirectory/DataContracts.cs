using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Dicom.ServiceModel;
using System;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory
{
    public static class ServerDirectoryNamespace
    {
        public const string Value = ImageViewerNamespace.Value + "/serverDirectory";
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

    //public class ServerExtensionDataContractAttribute : PolymorphicDataContractAttribute
    //{
    //    public ServerExtensionDataContractAttribute(string dataContractGuid)
    //        : base(dataContractGuid)
    //    {
    //    }
    //}

    //[KnownType("GetExtendedDataTypes")]
    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerData : DataContractBase
    {
        //public interface ITypeProvider
        //{
        //    IEnumerable<Type> GetTypes();
        //}

        //public sealed class ExtendedDataTypeProviderExtensionPoint : ExtensionPoint<ITypeProvider> { }

        [DataMember(IsRequired = true)]
        public bool IsPriorsServer { get; set; }

        //[DataMember(IsRequired = false)]
        //public IDictionary<object, object> Data { get; set; } 

        //public static Type[] GetExtendedDataTypes()
        //{
        //    try
        //    {
        //        return new ExtendedDataTypeProviderExtensionPoint().CreateExtensions()
        //                    .Cast<ITypeProvider>().SelectMany(t => t.GetTypes()).ToArray();
        //    }
        //    catch (NotSupportedException)
        //    {
        //    }

        //    return new Type[0];
        //}
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class ServerDirectoryEntry : DataContractBase
    {
        private ServerData _data;

        public ServerDirectoryEntry()
        {
        }

        public ServerDirectoryEntry(ApplicationEntity server)
        {
            Server = server;
        }

        [DataMember(IsRequired = true)]
        public ApplicationEntity Server { get; set; }

        [DataMember(IsRequired = false)]
        public ServerData Data
        {
            get { return _data ?? (_data = new ServerData()); }
            set { _data = value; }
        }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class GetServersResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public List<ServerDirectoryEntry> ServerEntries { get; set; }
    }

    #endregion

    #region Update

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class UpdateServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    #endregion

    #region Delete

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class DeleteServerRequest : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
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
        public ServerDirectoryEntry ServerEntry { get; set; }
    }

    [DataContract(Namespace = ServerDirectoryNamespace.Value)]
    public class AddServerResult : DataContractBase
    {
        [DataMember(IsRequired = true)]
        public ServerDirectoryEntry ServerEntry { get; set; }
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
