#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A collection of <see cref="IGraphic"/> objects.
	/// </summary>
	public class GraphicCollection : ObservableList<IGraphic>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="GraphicCollection"/>.
		/// </summary>
		internal GraphicCollection()
		{

		}
	}
}
