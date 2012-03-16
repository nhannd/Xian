#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// A context used when applying rules and actions within the ImageServer.
    /// </summary>
    /// <remarks>
    /// This class is used to pass information to rules and to the action procesor
    /// when applying rules within the ImageServer.  It should contain enough
    /// information to apply a given Action for a rule.
    /// </remarks>
    /// <seealso cref="ServerRulesEngine"/>
    public class ServerActionContext : ActionContext
	{
        #region Constructors

        public ServerActionContext(DicomMessageBase msg, ServerEntityKey filesystemKey,
                                   ServerPartition partition, ServerEntityKey studyLocationKey)
        {
            Message = msg;
            ServerPartitionKey = partition.Key;
            StudyLocationKey = studyLocationKey;
            FilesystemKey = filesystemKey;
        	ServerPartition = partition;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The partition of the object.
        /// </summary>
        public ServerEntityKey ServerPartitionKey { get; private set; }      

		/// <summary>
		/// The key of the filesystem being worked with..
		/// </summary>
        public ServerEntityKey FilesystemKey { get; private set; } 

		/// <summary>
		/// The study location key.
		/// </summary>
        public ServerEntityKey StudyLocationKey { get; private set; }  

		/// <summary>
		/// The server partition itself.
		/// </summary>
    	public ServerPartition ServerPartition { get; private set; }  

        #endregion
    }
}
