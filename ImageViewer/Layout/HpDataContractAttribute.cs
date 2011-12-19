#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Layout
{
	/// <summary>
	/// Identifies a class as a data-contract for HP serialization, and assigns it a GUID.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public class HpDataContractAttribute : Attribute
	{
		private readonly string _dataContractGuid;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dataContractGuid"></param>
		public HpDataContractAttribute(string dataContractGuid)
		{
			_dataContractGuid = dataContractGuid;
		}

		/// <summary>
		/// Gets the ID that identifies the data-contract.
		/// </summary>
		public Guid ContractId
		{
			get { return new Guid(_dataContractGuid); }
		}
	}
}
