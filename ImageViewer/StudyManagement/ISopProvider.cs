#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface for providers of <see cref="Sop"/> objects that are not necessarily <see cref="ImageSop"/>s.
	/// </summary>
	/// <remarks>
	/// See <see cref="ImageSop"/> for a more detailed explanation of 'providers'.
	/// </remarks>
	public interface ISopProvider
	{
		/// <summary>
		/// Gets a <see cref="Sop"/>.
		/// </summary>
		Sop Sop { get; }
	}
}
