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
	/// An <see cref="IDataLut"/> that is purely generated.
	/// </summary>
	/// <seealso cref="IDataLut"/>
	public interface IGeneratedDataLut : IDataLut
	{
		/// <summary>
		/// Called by the framework to release any data held by the lut; the Lut should be capable
		/// of recreating the data when it is needed.
		/// </summary>
		void Clear();
	}
}
