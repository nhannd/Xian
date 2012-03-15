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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class DiagnosticServiceDetail : DataContractBase
    {
		public DiagnosticServiceDetail()
		{
			this.ProcedureTypes = new List<ProcedureTypeSummary>();
		}

        public DiagnosticServiceDetail(EntityRef diagnosticServiceRef, string id, string name, List<ProcedureTypeSummary> procedureTypes,
			bool deactivated)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
            this.Id = id;
            this.Name = name;
            this.ProcedureTypes = procedureTypes;
        	this.Deactivated = deactivated;
        }

        [DataMember]
        public EntityRef DiagnosticServiceRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

        [DataMember]
        public List<ProcedureTypeSummary> ProcedureTypes;

		[DataMember]
		public bool Deactivated;

		public DiagnosticServiceSummary GetSummary()
        {
            return new DiagnosticServiceSummary(this.DiagnosticServiceRef, this.Id, this.Name, this.Deactivated);
        }
    }
}
