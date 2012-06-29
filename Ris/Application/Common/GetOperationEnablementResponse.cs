#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class GetOperationEnablementResponse : DataContractBase
	{
		public GetOperationEnablementResponse(Dictionary<string, bool> dictionary)
		{
			this.OperationEnablementDictionary = dictionary;
		}

		[DataMember]
		public Dictionary<string, bool> OperationEnablementDictionary;
	}
}
