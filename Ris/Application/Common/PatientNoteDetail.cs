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
    public class PatientNoteDetail : DataContractBase, ICloneable
    {
        public PatientNoteDetail(
			EntityRef patientNoteRef,
			string comment, 
            PatientNoteCategorySummary category, 
            StaffSummary createdBy, 
            DateTime? creationTime,
            DateTime? validRangeFrom,
            DateTime? validRangeUntil,
            bool isExpired)
        {
        	this.PatientNoteRef = patientNoteRef;
            this.Comment = comment;
            this.Category = category;
            this.Author = createdBy;
            this.CreationTime = creationTime;
            this.ValidRangeFrom = validRangeFrom;
            this.ValidRangeUntil = validRangeUntil;
            this.IsExpired = isExpired;
        }

        public PatientNoteDetail()
        {
        }

		[DataMember]
    	public EntityRef PatientNoteRef;

        [DataMember]
        public string Comment;

        [DataMember]
        public PatientNoteCategorySummary Category;

        [DataMember]
        public StaffSummary Author;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        [DataMember]
        public bool IsExpired;

        #region ICloneable Members

        public object Clone()
        {
            return new PatientNoteDetail(this.PatientNoteRef,this.Comment, this.Category, this.Author, this.CreationTime,
				this.ValidRangeFrom, this.ValidRangeUntil, this.IsExpired);
        }

        #endregion    
    }
}
