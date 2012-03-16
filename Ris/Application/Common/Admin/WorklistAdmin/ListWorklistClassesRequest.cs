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

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class ListWorklistClassesRequest : DataContractBase
    {
        /// <summary>
        /// Filters the results by the specified class names.
        /// </summary>
        [DataMember]
        public List<string> ClassNames;

        /// <summary>
        /// Filters results by the specified categories.
        /// </summary>
        [DataMember]
        public List<string> Categories;

        /// <summary>
        /// Specifies whether static worklist classes should be included in the results.
        /// </summary>
        [DataMember]
        public bool IncludeStatic;
    }
}
