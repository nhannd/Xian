#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClearCanvas.Common.Configuration
{
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
        /// Obtains the specified document for the specified user and instance key.  If user is null,
        /// the shared document is obtained.
        /// </summary>
        /// <remarks>
        /// Implementors should throw a <see cref="ConfigurationDocumentNotFoundException"/> if the requested document does not exist.
        /// </remarks>
        /// <exception cref="ConfigurationDocumentNotFoundException">The requested document does not exist.</exception>
        TextReader GetDocument(string name, Version version, string user, string instanceKey);

        /// <summary>
        /// Stores the specified document for the current user and instance key.  If user is null,
        /// the document is stored as a shared document.
        /// </summary>
        void PutDocument(string name, Version version, string user, string instanceKey, TextReader content);
    }
}
