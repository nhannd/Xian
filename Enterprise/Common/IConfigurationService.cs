using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Defines a service for saving/retrieving configuration data to/from a persistent store.
    /// </summary>
    [ServiceContract]
    public interface IConfigurationService : ICoreServiceLayer
    {
        /// <summary>
        /// Loads the document specified by the name, version, user and instance key, returning the document text as a string.
        /// The user and instance key may be null.  If the document does not exist, an <see cref="ConfigurationDocumentNotFoundException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConfigurationDocumentNotFoundException))]
        string LoadDocument(string name, Version version, string user, string instanceKey);

        /// <summary>
        /// Stores the document for the specified group, version, user and instance key.  The user and instance key may be null.
        /// If the document does not exist, it will be created.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <param name="documentText"></param>
        [OperationContract]
        void SaveDocument(string name, Version version, string user, string instanceKey, string documentText);

        /// <summary>
        /// Removes the specified document, if it exists.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        [OperationContract]
        [FaultContract(typeof(ConfigurationDocumentNotFoundException))]
        void RemoveDocument(string name, Version version, string user, string instanceKey);

    }
}
