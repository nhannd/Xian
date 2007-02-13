using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
	public delegate void BlockingOperationDelegate();

	public interface IBlockingOperationView : IView
	{
		void Run(BlockingOperationDelegate operation);
	}
}
