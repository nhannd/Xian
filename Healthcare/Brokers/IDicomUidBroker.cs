#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	/// <summary>
	/// Custom broker that provides DICOM UIDs.
	/// </summary>
	public interface IDicomUidBroker : IPersistenceBroker
	{
		/// <summary>
		/// Gets a new DICOM UID.
		/// </summary>
		/// <returns></returns>
		string GetNewUid();
	}
}
