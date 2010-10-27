#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
    /// <summary>
    /// An interface for a time provider.
    /// </summary>
    /// <remarks>
    /// The framework provides an <see cref="ITimeProvider"/> (<see cref="Platform.Time"/>),
    /// which uses the <see cref="TimeProviderExtensionPoint"/> internally.
    /// </remarks>
	public interface ITimeProvider
    {
		/// <summary>
		/// Gets the current date and time.
		/// </summary>
		DateTime CurrentTime { get; }
    }
}
