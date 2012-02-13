#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Identifies a configuration document.
	/// </summary>
	[DataContract]
	public class ConfigurationDocumentKey : IDefinesCacheKey
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="documentName"></param>
		/// <param name="version"></param>
		/// <param name="user"></param>
		/// <param name="instanceKey"></param>
		public ConfigurationDocumentKey(string documentName, Version version, string user, string instanceKey)
		{
			DocumentName = documentName;
			Version = version;
			User = user;
			InstanceKey = instanceKey;
		}

		/// <summary>
		/// Gets the name of the document.
		/// </summary>
		[DataMember]
		public string DocumentName { get; private set; }

		/// <summary>
		/// Gets the version of the document.
		/// </summary>
		[DataMember]
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the owner of the document.
		/// </summary>
		[DataMember]
		public string User { get; private set; }

		/// <summary>
		/// Gets the instance key of the document.
		/// </summary>
		[DataMember]
		public string InstanceKey { get; private set; }

		#region IDefinesCacheKey Members

		/// <summary>
		/// Gets the cache key defined by this instance.
		/// </summary>
		/// <returns></returns>
		string IDefinesCacheKey.GetCacheKey()
		{
			return string.Format("{0}:{1}:{2}:{3}",
				this.DocumentName,
				this.Version,
				StringUtilities.EmptyIfNull(this.User),
				StringUtilities.EmptyIfNull(this.InstanceKey));
		}

		#endregion
	}
}
