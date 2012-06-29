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
    public class StaffGroupDetail : DataContractBase
    {
        public StaffGroupDetail()
        {
            this.Members = new List<StaffSummary>();
			this.Worklists = new List<WorklistSummary>();
        }

        public StaffGroupDetail(EntityRef groupRef, string name, string description, bool isElective, List<StaffSummary> members, List<WorklistSummary> worklists, bool deactivated)
        {
            this.StaffGroupRef = groupRef;
            this.Name = name;
            this.Description = description;
            this.Members = members;
        	this.Worklists = worklists;
        	this.IsElective = isElective;
        	this.Deactivated = deactivated;
        }

        [DataMember]
        public EntityRef StaffGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

		[DataMember]
		public bool IsElective;

		[DataMember]
        public List<StaffSummary> Members;

    	[DataMember]
		public List<WorklistSummary> Worklists;

		[DataMember]
		public bool Deactivated;
    }
}