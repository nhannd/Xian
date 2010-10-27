#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface to an <see cref="ISopDataSource"/> whose internal
	/// storage structure is a <see cref="DicomMessageBase"/>.
	/// </summary>
	/// <remarks>
	/// This interface should be considered unstable and may be removed in a future
	/// version.  See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
	/// </remarks>
	public interface IDicomMessageSopDataSource : ISopDataSource
	{
		//TODO (later): remove due to thread safety issues.

		/// <summary>
		/// Gets the source <see cref="DicomMessageBase"/>.
		/// </summary>
		/// <remarks>
		/// See the remarks for <see cref="IDicomMessageSopDataSource"/>.
		/// Accessing this property is entirely unsafe, as the framework performs
		/// a lot of asynchronous operations on the <see cref="ISopDataSource"/> objects.
		/// All implementations of <see cref="ISopDataSource"/> must be thread-safe, and
		/// this property cannot be, as it directly exposes an underlying data structure.
		/// This property will likely be removed in a future version due to thread-safety concerns.
		/// </remarks>
		DicomMessageBase SourceMessage { get; }
	}
}