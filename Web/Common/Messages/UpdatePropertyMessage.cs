#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Web.Common.Messages
{
	[DataContract(Namespace = Namespace.Value)]
	public class UpdatePropertyMessage : Message
	{
        [DataMember(IsRequired = true)]
        public string MimeType { get; set; }
        
        [DataMember(IsRequired = false)]
		public string PropertyName { get; set; }

		[DataMember(IsRequired = false)]
		public object Value { get; set; }
	}
}