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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Extended.Common.OrderNotes
{
    [DataContract]
	public class QueryNoteboxRequest : PagedDataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteboxClass"></param>
        /// <param name="queryCount"></param>
        /// <param name="queryItems"></param>
        public QueryNoteboxRequest(string noteboxClass, bool queryCount, bool queryItems)
        {
            NoteboxClass = noteboxClass;
            QueryCount = queryCount;
            QueryItems = queryItems;
        }

        /// <summary>
        /// Specified the notebox class to query.
        /// </summary>
        [DataMember]
        public string NoteboxClass;

		/// <summary>
		/// Identifies the staff group notebox to query, in the case where <see cref="NoteboxClass"/> 
		/// refers to a group notebox.
		/// </summary>
		[DataMember]
    	public EntityRef StaffGroupRef;

        /// <summary>
        /// Specifies whether to return a count of the total number of items in the notebox.
        /// </summary>
        [DataMember]
        public bool QueryCount;

        /// <summary>
        /// Specifies whether to return the list of items in the notebox.
        /// </summary>
        [DataMember]
        public bool QueryItems;
    }
}
