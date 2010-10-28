#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
    [DataContract]
    public class ListCannedTextForUserRequest : ListRequestBase
    {
        public ListCannedTextForUserRequest()
        {
        }

        public ListCannedTextForUserRequest(SearchResultPage page)
            :base(page)
        {
        }

		/// <summary>
		/// Name filter, exact match. Optional.
		/// </summary>
        [DataMember]
        public string Name;
    }
}
