using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class QueryNoteboxRequest : DataContractBase
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
