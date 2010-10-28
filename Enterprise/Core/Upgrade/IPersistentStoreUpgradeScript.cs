#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core.Upgrade
{
	/// <summary>
	/// Interface representing an upgrade script for a PersistentStore.
	/// </summary>
	public interface IPersistentStoreUpgradeScript
	{
		/// <summary>
		/// The PersistentStore version for which the script upgrades from.
		/// </summary>
		Version SourceVersion { get; }
		/// <summary>
		/// The resultant PersistentStore version after the script has been run.
		/// </summary>
		Version DestinationVersion { get; }
		/// <summary>
		/// Execute the upgrade script.
		/// </summary>
		void Execute();
	}
}
