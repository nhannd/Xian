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
	/// Interface to an <see cref="ISopDataSource"/> whose internal source is
	/// a local <see cref="DicomFile"/>.
	/// </summary>
	public interface ILocalSopDataSource : IDicomMessageSopDataSource
	{
		//TODO (later): remove due to thread safety issues.
		
		/// <summary>
		/// Gets the source <see cref="DicomFile"/>.
		/// </summary>
		/// <remarks>See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
		/// This property will likely be removed in a future version due to thread-safety concerns.</remarks>
		DicomFile File { get; } 

		/// <summary>
		/// Gets the filename of the source <see cref="DicomFile"/>.
		/// </summary>
		string Filename { get; }
	}
}
