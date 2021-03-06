﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Common.Serialization
{
	/// <summary>
	/// Base class for all objects that serve as WCF data contracts.
	/// </summary>
	[DataContract]
	public abstract class DataContractBase : IExtensibleDataObject
	{
		#region IExtensibleDataObject Members

		public ExtensionDataObject ExtensionData { get; set; }

		#endregion
	}
}