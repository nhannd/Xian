#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A read-only Linear Voi Lut, where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/> cannot be set.
	/// </summary>
	/// <seealso cref="IVoiLut"/>
	public interface IVoiLutLinear : IVoiLut
	{
		/// <summary>
		/// Gets the Window Width.
		/// </summary>
		double WindowWidth { get; }

		/// <summary>
		/// Gets the Window Center.
		/// </summary>
		double WindowCenter { get; }
	}
}
