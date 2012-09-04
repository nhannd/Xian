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
    public class DiagnosticServiceSummary : DataContractBase, IEquatable<DiagnosticServiceSummary>
    {
        public DiagnosticServiceSummary(EntityRef diagnosticServiceRef, string id, string name, bool deactivated)
        {
            this.DiagnosticServiceRef = diagnosticServiceRef;
            this.Id = id;
            this.Name = name;
        	this.Deactivated = deactivated;
        }

		public DiagnosticServiceSummary()
		{
		}

        [DataMember]
        public EntityRef DiagnosticServiceRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

		[DataMember]
		public bool Deactivated;

        public bool Equals(DiagnosticServiceSummary diagnosticServiceSummary)
        {
            if (diagnosticServiceSummary == null) return false;
            return Equals(DiagnosticServiceRef, diagnosticServiceSummary.DiagnosticServiceRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as DiagnosticServiceSummary);
        }

        public override int GetHashCode()
        {
            return DiagnosticServiceRef.GetHashCode();
        }
    }
}
