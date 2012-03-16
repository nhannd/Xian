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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ReportDetail : DataContractBase
    {
        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ReportStatus;

		/// <summary>
		/// This may not contains all the parts that are in a report.
		/// The cancelled reports may not be included.
		/// </summary>
        [DataMember]
        public List<ReportPartDetail> Parts;

		/// <summary>
		/// Gets the procedures associated with the report.
		/// </summary>
        [DataMember]
        public List<ProcedureDetail> Procedures;

		/// <summary>
		/// Return the report part correspond to the report part index
		/// </summary>
		/// <param name="reportPartIndex">The report part index, not the array index of the list of Parts</param>
		/// <returns></returns>
        public ReportPartDetail GetPart(int reportPartIndex)
        {
			if (this.Parts == null || reportPartIndex < 0)
                return null;

			return CollectionUtils.SelectFirst(this.Parts,
				delegate(ReportPartDetail detail) { return detail.Index.Equals(reportPartIndex); });
        }
    }
}