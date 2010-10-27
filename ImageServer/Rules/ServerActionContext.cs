#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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
    public class ServerActionContext
	{
		#region Private Members
        private DicomMessageBase _msg;
        private readonly ServerEntityKey _serverPartitionKey;
        private readonly ServerEntityKey _studyLocationKey;
        private readonly ServerEntityKey _filesystemKey;
    	private readonly ServerPartition _partition;
    	private ServerCommandProcessor _commandProcessor;
        #endregion

        #region Constructors

        public ServerActionContext(DicomMessageBase msg, ServerEntityKey filesystemKey,
                                   ServerPartition partition, ServerEntityKey studyLocationKey)
        {
            _msg = msg;
            _serverPartitionKey = partition.Key;
            _studyLocationKey = studyLocationKey;
            _filesystemKey = filesystemKey;
        	_partition = partition;
        }

        #endregion

        #region Public Properties
		/// <summary>
		/// The message being worked against.
		/// </summary>
        public DicomMessageBase Message
        {
            get { return _msg; }
			set { _msg = value; }
        }

		/// <summary>
		/// The command processor.
		/// </summary>
        public ServerCommandProcessor CommandProcessor
        {
            get { return _commandProcessor; }
            set { _commandProcessor = value; }
        }

		/// <summary>
		/// The partition of the object.
		/// </summary>
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
        }

		/// <summary>
		/// The key of the filesystem being worked with..
		/// </summary>
        public ServerEntityKey FilesystemKey
        {
            get { return _filesystemKey; }
        }

		/// <summary>
		/// The study location key.
		/// </summary>
        public ServerEntityKey StudyLocationKey
        {
            get { return _studyLocationKey; }
        }

		/// <summary>
		/// The server partition itself.
		/// </summary>
    	public ServerPartition ServerPartition
    	{
			get { return _partition; }
    	}
        #endregion
    }
}
