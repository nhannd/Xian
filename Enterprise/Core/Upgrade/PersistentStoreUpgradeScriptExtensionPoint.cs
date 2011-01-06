#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Upgrade
{
	/// <summary>
	/// Extension point for PersistentStore upgrade scripts that implement <see cref="IPersistentStoreUpgradeScript"/>.
	/// </summary>
	[ExtensionPoint]
	public class PersistentStoreUpgradeScriptExtensionPoint : ExtensionPoint<IPersistentStoreUpgradeScript>
	{
	}
}
