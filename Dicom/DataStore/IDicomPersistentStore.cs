#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Dicom.DataStore
{
    /// <summary>
    /// Persists dicom data to the Data Store.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any failure during a call to <see cref="UpdateSopInstance"/> should be considered a failure for the entire
    /// transaction and should not be committed (via <see cref="Commit"/>).
	/// </para>
	/// <para>
    /// Although internally, <see cref="UpdateSopInstance"/> uses <see cref="IDicomPersistentStoreValidator"/>, it is
    /// only to ensure that bad data does not get into the Data Store.  You should use the <see cref="IDicomPersistentStoreValidator"/>
    /// ahead of time to rule out unacceptable sops before calling <see cref="UpdateSopInstance"/>.  This will increase
    /// the likelihood that batch updates will succeed, containing only valid data.
	/// </para>
    /// </remarks>
	public interface IDicomPersistentStore : IDisposable
    {
		/// <summary>
		/// Adds/Updates a Sop Instance in the data store.
		/// </summary>
    	void UpdateSopInstance(DicomFile dicomFile);
    	
		/// <summary>
		/// Commits a batch set of updates, from previous calls to <see cref="UpdateSopInstance"/>.
		/// </summary>
		void Commit();
    }
}
