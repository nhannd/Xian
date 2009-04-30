#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteSummary : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderNoteRef"></param>
        /// <param name="category"></param>
        /// <param name="creationTime"></param>
        /// <param name="postTime"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOfGroup"></param>
        /// <param name="isAcknowledged"></param>
		/// <param name="urgent"></param>
		/// <param name="noteBody"></param>
        public OrderNoteSummary(EntityRef orderNoteRef, string category, DateTime? creationTime, DateTime? postTime,
			StaffSummary author, StaffGroupSummary onBehalfOfGroup, bool isAcknowledged, bool urgent, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
        	OnBehalfOfGroup = onBehalfOfGroup;
            IsAcknowledged = isAcknowledged;
        	Urgent = urgent;
            NoteBody = noteBody;
        }

        [DataMember]
        public EntityRef OrderNoteRef;

        [DataMember]
        public string Category;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public DateTime? PostTime;

        [DataMember]
        public StaffSummary Author;

		[DataMember]
		public StaffGroupSummary OnBehalfOfGroup;

        /// <summary>
        /// Gets a value indicating whether the note has been acknowledged by all recipients.
        /// </summary>
        [DataMember]
        public bool IsAcknowledged;

        [DataMember]
        public bool Urgent;

        [DataMember]
        public string NoteBody;
    }
}
