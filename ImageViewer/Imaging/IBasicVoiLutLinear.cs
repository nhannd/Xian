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
	/// The most basic of Linear Luts where the <see cref="WindowWidth"/> and <see cref="WindowCenter"/>
	/// can be set/manipulated.
	/// </summary>
	/// <seealso cref="IVoiLutLinear"/>
	public interface IBasicVoiLutLinear : IVoiLutLinear
	{
		/// <summary>
		/// Gets or sets the Window Width.
		/// </summary>
		new double WindowWidth { get; set; }

		/// <summary>
		/// Gets or sets the Window Center.
		/// </summary>
		new double WindowCenter { get; set; }
	}
}
