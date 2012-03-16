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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class LoadVisitForEditResponse : DataContractBase
    {
        public LoadVisitForEditResponse(EntityRef visitRef, VisitDetail visitDetail)
        {
            this.VisitRef = visitRef;
            this.VisitDetail = visitDetail;
        }

        [DataMember]
        public EntityRef VisitRef;

        [DataMember]
        public EntityRef Patient;

        [DataMember]
        public VisitDetail VisitDetail;
    }
}
