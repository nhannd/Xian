#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Defines the interface for finding related (or 'prior') studies
	/// based on the studies that are already loaded in an <see cref="IImageViewer"/>'s <see cref="StudyTree"/>.
	/// </summary>
	/// <remarks>
	/// The <see cref="ImageViewerComponent"/> internally uses an <see cref="IPriorStudyFinder"/> from within
	/// it's <see cref="IPriorStudyLoader"/> to find and load prior related studies into the <see cref="ImageViewerComponent"/>.
	/// </remarks>
	public interface IPriorStudyFinder
	{
		/// <summary>
		/// Sets the <see cref="IImageViewer"/> for which prior studies are to found (and added/loaded).
		/// </summary>
		void SetImageViewer(IImageViewer viewer);

		/// <summary>
		/// Gets the list of prior studies.
		/// </summary>
		/// <returns></returns>
		StudyItemList FindPriorStudies();

		/// <summary>
		/// Cancels the search for prior studies.
		/// </summary>
		void Cancel();
	}
}
