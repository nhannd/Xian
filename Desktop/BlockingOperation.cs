#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for views onto <see cref="BlockingOperation"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class BlockingOperationViewExtensionPoint : ExtensionPoint<IBlockingOperationView>
	{
	}

	/// <summary>
	/// The BlockingOperation class is a static class that allows application level code to
	/// use a wait cursor without having to explicitly reference a particular Gui Toolkit's API.
	/// </summary>
	public static class BlockingOperation
	{
		/// <summary>
		/// Executes the provided operation in the view, showing a wait cursor for the duration of the call.
		/// </summary>
		/// <param name="operation">The operation to execute in the view layer.</param>
		public static void Run(BlockingOperationDelegate operation)
		{
			Platform.CheckForNullReference(operation, "operation");

			IBlockingOperationView operationView = null;

			try
			{
				operationView = (IBlockingOperationView)ViewFactory.CreateView(new BlockingOperationViewExtensionPoint());
			}
			catch (Exception e)
			{
                Platform.Log(LogLevel.Error, e);
			}

			if (operationView == null)
			{
				operation();
			}
			else
			{
				operationView.Run(operation);
			}
		}
	}
}
