#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System;

namespace ClearCanvas.Web.Common
{
	[DataContract(Namespace = Namespace.Value)]
    public abstract class Application : DataContractBase
	{
		[DataMember(IsRequired = true)]
		public Guid Identifier { get; set; }
	}
}
