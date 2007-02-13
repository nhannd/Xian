using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for views onto <see cref="BlockingOperation"/>.
	/// </summary>
	[ExtensionPoint]
	public class BlockingOperationViewExtensionPoint : ExtensionPoint<IBlockingOperationView>
	{
	}

	/// <summary>
	/// The BlockingOperation class is a static class that allows application level code to
	/// use a wait cursor without having to explicitly reference a particular Gui Toolkit's API.
	/// </summary>
	public static class BlockingOperation
	{
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
				Platform.Log(e);
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
