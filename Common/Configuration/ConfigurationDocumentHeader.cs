#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Encapsulates meta-data about a configuration document.
	/// </summary>
	[DataContract]
	public class ConfigurationDocumentHeader
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="creationTime"></param>
		/// <param name="modifiedTime"></param>
		public ConfigurationDocumentHeader(ConfigurationDocumentKey key, DateTime creationTime, DateTime modifiedTime)
		{
			Key = key;
			CreationTime = creationTime;
			ModifiedTime = modifiedTime;
		}

		/// <summary>
		/// Gets the key that identifies the document.
		/// </summary>
		[DataMember]
		public ConfigurationDocumentKey Key { get; private set; }

		/// <summary>
		/// Gets the document creation time.
		/// </summary>
		[DataMember]
		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets the document last modification time.
		/// </summary>
		[DataMember]
		public DateTime ModifiedTime { get; private set; }
	}
}
