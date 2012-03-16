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

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [DataContract]
    public class EnumerationSummary : DataContractBase
    {
        public EnumerationSummary()
        {

        }

        public EnumerationSummary(string assemblyQualifiedClassName, string displayName, bool canAddRemoveValues)
        {
            this.AssemblyQualifiedClassName = assemblyQualifiedClassName;
            this.DisplayName = displayName;
            this.CanAddRemoveValues = canAddRemoveValues;
        }

        [DataMember]
        public string DisplayName;

        [DataMember]
        public string AssemblyQualifiedClassName;

        [DataMember]
        public bool CanAddRemoveValues;
    }
}
