#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// A delegate to be executed in the view.
	/// </summary>
	public delegate void BlockingOperationDelegate();

	/// <summary>
	/// An interface for executing long-running operations in the
	/// view while showing a wait cursor.
	/// </summary>
	public interface IBlockingOperationView : IView
	{
		/// <summary>
		/// Executes the specified operation in the view, showing a wait cursor.
		/// </summary>
		void Run(BlockingOperationDelegate operation);
	}
}
