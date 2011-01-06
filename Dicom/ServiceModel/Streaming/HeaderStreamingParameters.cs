#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Encapsulates the parameters passed by the client to header streaming service.
    /// </summary>
    /// <remarks>
    /// <see cref="HeaderStreamingParameters"/> is passed to the service that implements <see cref="IHeaderStreamingService"/> 
    /// when the client wants to retrieve header information of a study. The study is identified by the <see cref="StudyInstanceUID"/>
    /// and the <see cref="ServerAETitle"/> where it is located.
    /// </remarks>
    [DataContract]
    public class HeaderStreamingParameters
    {
        #region Private members

        private string _serverAETitle;
        private string _studyInstanceUID;
        private string _referenceID;        
        #endregion Private members

        #region Public Properties

        /// <summary>
        /// Study instance UID of the study whose header will be retrieved.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        /// <summary>
        /// AE title of the server where the study is located.
        /// </summary>
        [DataMember(IsRequired=true)]
        public string ServerAETitle
        {
            get { return _serverAETitle; }
            set { _serverAETitle = value; }
        }

        /// <summary>
        /// A ticket for tracking purposes.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ReferenceID
        {
            get { return _referenceID; }
            set { _referenceID = value; }
        }

        #endregion Public Properties
    }
}