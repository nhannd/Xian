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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ModalityProcedureStepTypeDetail : DataContractBase
    {
        public ModalityProcedureStepTypeDetail(string id, string name, ModalityDetail defaultModality)
        {
            this.Id = id;
            this.Name = name;
            this.DefaultModality = defaultModality;
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public ModalityDetail DefaultModality;
    }
}
