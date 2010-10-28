#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.DataStore
{
	/// <summary>
	/// Provides a way to determine if a dicom dataset is valid for insertion into the 
	/// data store before attempting to insert it.
	/// </summary>
	/// <remarks>
	/// The implementation of this interface is not guaranteed to be thread-safe.
	/// </remarks>
	/// <seealso cref="IDicomPersistentStore"/>
	public interface IDicomPersistentStoreValidator
	{
		/// <summary>
		/// Validates the input data and insures it is appropriate for insertion into the datastore.
		/// </summary>
		/// <remarks>An exception is thrown when the data is invalid.</remarks>
		void Validate(DicomFile file);
	}
}
