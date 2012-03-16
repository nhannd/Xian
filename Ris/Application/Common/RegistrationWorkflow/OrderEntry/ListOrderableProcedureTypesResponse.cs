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

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ListOrderableProcedureTypesResponse : DataContractBase
    {
        public ListOrderableProcedureTypesResponse(List<ProcedureTypeSummary> orderableProcedureTypes)
        {
            this.OrderableProcedureTypes = orderableProcedureTypes;
        }

        /// <summary>
        /// The set of procedure types that can be additionally ordered.
        /// </summary>
        [DataMember]
        public List<ProcedureTypeSummary> OrderableProcedureTypes;
    }
}
