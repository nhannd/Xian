#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    /// <summary>
    /// Requests a list of worklist, according to the specified filters.
    /// </summary>
    [DataContract]
    public class ListWorklistsRequest : ListRequestBase
    {
        /// <summary>
        /// Filters the results by the specified class names.
        /// </summary>
        [DataMember]
        public List<string> ClassNames;

        /// <summary>
        /// Filters the results by the specified categories.
        /// </summary>
        [DataMember]
        public List<string> Categories;

        /// <summary>
        /// Specifies whether static worklists should be returned in the results.
        /// </summary>
        [DataMember]
        public bool IncludeStatic;

        /// <summary>
        /// Specifies whether user and group owned worklists should be returned in the results.
        /// </summary>
        [DataMember]
        public bool IncludeUserDefinedWorklists;

        /// <summary>
        /// Filters the results by the specified name.
        /// </summary>
        [DataMember]
        public string WorklistName;
    }
}