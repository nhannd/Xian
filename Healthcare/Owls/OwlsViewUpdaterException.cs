#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Healthcare.Owls
{
	/// <summary>
	/// Represents an exception condition encountered by a <see cref="IViewUpdater"/> class.
	/// </summary>
	public class OwlsViewUpdaterException : Exception
	{
		public OwlsViewUpdaterException(string message)
			:base(message)
		{
		}
	}
}
