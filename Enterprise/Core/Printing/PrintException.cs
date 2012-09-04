#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;


namespace ClearCanvas.Enterprise.Core.Printing
{
	public class PrintException : Exception
	{
		public PrintException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public PrintException(string message) : base(message)
		{
		}
	}
}
