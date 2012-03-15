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

namespace ClearCanvas.Enterprise.Common
{
    [DataContract]
    public abstract class PagedDataContractBase : DataContractBase
    {
        public PagedDataContractBase()
        {
            this.Page = new SearchResultPage();
        }

        public PagedDataContractBase(SearchResultPage page)
        {
            this.Page = page;
        }

        [DataMember]
        public SearchResultPage Page;
    }
}
