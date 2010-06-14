#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Common.Caching;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class QueryWorklistRequest : PagedDataContractBase, IDefinesCacheKey
	{
		public QueryWorklistRequest(string worklistType, bool queryItems, bool queryCount, bool downtimeRecoveryMode, EntityRef workingFacilityRef)
		{
			this.WorklistClass = worklistType;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.DowntimeRecoveryMode = downtimeRecoveryMode;
			this.WorkingFacilityRef = workingFacilityRef;
		}

		public QueryWorklistRequest(EntityRef worklistRef, bool queryItems, bool queryCount, bool downtimeRecoveryMode, EntityRef workingFacilityRef)
		{
			this.WorklistRef = worklistRef;
			this.QueryItems = queryItems;
			this.QueryCount = queryCount;
			this.DowntimeRecoveryMode = downtimeRecoveryMode;
			this.WorkingFacilityRef = workingFacilityRef;
		}

		/// <summary>
		/// Specifies the worklist instance to query, for instanced worklists.
		/// </summary>
		[DataMember]
		public EntityRef WorklistRef;

		/// <summary>
		/// Specifies the class of worklist to query, for singleton worklists.
		/// </summary>
		[DataMember]
		public string WorklistClass;

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

		/// <summary>
		/// Specifies whether to return the list of items in the notebox.
		/// </summary>
		[DataMember]
		public bool DowntimeRecoveryMode;

		/// <summary>
		/// Specifies the working facility.
		/// </summary>
		[DataMember]
		public EntityRef WorkingFacilityRef;

		string IDefinesCacheKey.GetCacheKey()
		{
			// important to include working facility in the cache key, because this affects the response
			var r = this.WorklistRef == null ? this.WorklistClass : this.WorklistRef.ToString(true, false);
			var f = this.WorkingFacilityRef == null ? "" : this.WorkingFacilityRef.ToString(false, false);
			return string.Format("{0}:{1}:{2}:{3}:{4}", r, f, QueryCount, QueryItems, DowntimeRecoveryMode);
		}
	}
}
