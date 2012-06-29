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
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PatientNoteCategorySummary : DataContractBase, ICloneable, IEquatable<PatientNoteCategorySummary>
    {
        public PatientNoteCategorySummary(EntityRef noteCategoryRef, string name, string description, EnumValueInfo severity, bool deactivated)
        {
            this.NoteCategoryRef = noteCategoryRef;
            this.Name = name;
            this.Description = description;
            this.Severity = severity;
        	this.Deactivated = deactivated;
        }

        public PatientNoteCategorySummary()
        {
        }

        [DataMember]
        public EntityRef NoteCategoryRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

		[DataMember]
		public bool Deactivated;


        #region ICloneable Members

        public object Clone()
        {
			return new PatientNoteCategorySummary(this.NoteCategoryRef, this.Name, this.Description, this.Severity, this.Deactivated);
        }

        #endregion

        public bool Equals(PatientNoteCategorySummary patientNoteCategorySummary)
        {
            if (patientNoteCategorySummary == null) return false;
            return Equals(NoteCategoryRef, patientNoteCategorySummary.NoteCategoryRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PatientNoteCategorySummary);
        }

        public override int GetHashCode()
        {
            return NoteCategoryRef.GetHashCode();
        }
    }
}
