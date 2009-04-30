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

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class CannedTextSummary : DataContractBase
	{
		/// <summary>
		/// Define the maximum length of the TextSnippet
		/// </summary>
		public const int MaxTextLength = 128;

		public CannedTextSummary(EntityRef cannedTextRef
			, string name
			, string category
			, StaffSummary staff
			, StaffGroupSummary staffGroup
			, string textSnippet)
		{
			this.CannedTextRef = cannedTextRef;
			this.Name = name;
			this.Category = category;
			this.Staff = staff;
			this.StaffGroup = staffGroup;
			this.TextSnippet = textSnippet.Substring(0, textSnippet.Length < MaxTextLength ? textSnippet.Length : MaxTextLength);
		}

		[DataMember]
		public EntityRef CannedTextRef;

		[DataMember]
		public string Name;

		[DataMember]
		public string Category;

		[DataMember]
		public StaffSummary Staff;

		[DataMember]
		public StaffGroupSummary StaffGroup;

		[DataMember]
		public string TextSnippet;

		public bool IsPersonal
		{
			get { return this.Staff != null; }
		}

		public bool IsGroup
		{
			get { return this.StaffGroup != null; }
		}
	}
}
