#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolGroupDetail : DataContractBase
    {
        public ProtocolGroupDetail()
        {
			Codes = new List<ProtocolCodeSummary>();
            ReadingGroups = new List<ProcedureTypeGroupSummary>();
        }

		public ProtocolGroupDetail(string name, string description, List<ProtocolCodeSummary> codes, List<ProcedureTypeGroupSummary> readingGroups)
        {
            Name = name;
            Description = description;
            Codes = codes;
            ReadingGroups = readingGroups;
        }

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public List<ProtocolCodeSummary> Codes;

        [DataMember]
        public List<ProcedureTypeGroupSummary> ReadingGroups;
    }
}
