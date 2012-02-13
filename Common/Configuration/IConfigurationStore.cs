#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Represents a configuration document.
	/// </summary>
	public interface IConfigurationDocument
	{
		/// <summary>
		/// Gets the document header.
		/// </summary>
		ConfigurationDocumentHeader Header { get; }

		/// <summary>
		/// Gets the entire content of the document as a string.
		/// </summary>
		/// <returns></returns>
		string ReadAll();

		/// <summary>
		/// Gets a reader that can read the document content.
		/// </summary>
		/// <returns></returns>
		TextReader GetReader();
	}



	/// <summary>
	/// Defines the interface to a mechanism for the storage of configuration data.
	/// </summary>
	/// <remarks>
	/// This interface is more general purpose than <see cref="ISettingsStore"/>, in that it allows storage
	/// of arbitrary configuration "documents" that need not conform to any particular structure.
	/// </remarks>
	public interface IConfigurationStore
	{
		/// <summary>
		/// Lists documents in the configuration that match the specified query.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ConfigurationDocumentHeader> ListDocuments(ConfigurationDocumentQuery query);

		/// <summary>
		/// Retrieves the specified document.
		/// </summary>
		/// <exception cref="ConfigurationDocumentNotFoundException">The requested document does not exist.</exception>
		IConfigurationDocument GetDocument(ConfigurationDocumentKey documentKey);

		/// <summary>
		/// Stores the specified document.
		/// </summary>
		void PutDocument(ConfigurationDocumentKey documentKey, TextReader content);

		/// <summary>
		/// Stores the specified document.
		/// </summary>
		void PutDocument(ConfigurationDocumentKey documentKey, string content);
	}
}
