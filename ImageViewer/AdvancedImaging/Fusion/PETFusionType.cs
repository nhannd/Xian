#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	/// <summary>
	/// Specifies the type of PET series fusion operation.
	/// </summary>
	public enum PETFusionType
	{
		/// <summary>
		/// Specifies PET-CT Fusion.
		/// </summary>
		CT,

		/// <summary>
		/// Specifies PET-MRI Fusion.
		/// </summary>
		MR
	}
}