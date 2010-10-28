#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Common.Shreds
{
	/// <summary>
	/// Defines an extension point for shreds that can be loaded and executed by the shred-host.
	/// </summary>
    [ExtensionPoint()]
	public sealed class ShredExtensionPoint : ExtensionPoint<IShred>
    {
    }
}
