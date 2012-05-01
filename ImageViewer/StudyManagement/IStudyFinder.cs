#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO (later): move this stuff into ImageViewer.Common and deprecate it.

	/// <summary>
	/// Defines a study finder.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyFinder"/> abstracts the finding of studies,
	/// allowing many means of finding studies (e.g., local database,
	/// DICOM query, DICOMDIR, etc.) to be treated in the same way.
	/// </remarks>
    public interface IStudyFinder
    {
		/// <summary>
		/// Gets the name of the study finder.
		/// </summary>
        string Name { get; }

		/// <summary>
		/// Queries for studies on a target server matching the specified query parameters.
		/// </summary>
        StudyItemList Query(QueryParameters queryParams, IApplicationEntity targetServer);
    }
}
