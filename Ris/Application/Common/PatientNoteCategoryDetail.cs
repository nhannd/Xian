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
    public class PatientNoteCategoryDetail : DataContractBase, ICloneable
    {
        public PatientNoteCategoryDetail(EntityRef categoryRef, string category, string description, EnumValueInfo severity, bool deactivated)
        {
        	this.NoteCategoryRef = categoryRef;
            this.Category = category;
            this.Description = description;
            this.Severity = severity;
        	this.Deactivated = deactivated;
        }

        public PatientNoteCategoryDetail()
        {
        }

		[DataMember]
		public EntityRef NoteCategoryRef;

		[DataMember]
        public string Category;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Severity;

		[DataMember]
		public bool Deactivated;

        #region ICloneable Members

        public object Clone()
        {
        	return new PatientNoteCategoryDetail(this.NoteCategoryRef, this.Category, this.Description, this.Severity, this.Deactivated);
        }

        #endregion
    }
}
