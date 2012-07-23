#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin
{
    [DataContract]
    public class AddProtocolCodeResponse : DataContractBase
    {
        public AddProtocolCodeResponse(ProtocolCodeSummary summary)
        {
            ProtocolCode = summary;
        }

        [DataMember]
		public ProtocolCodeSummary ProtocolCode;
    }
}