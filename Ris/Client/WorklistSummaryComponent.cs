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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
	public class WorklistSummaryComponent : DHtmlComponent
	{
		[DataContract]
		internal class WorklistSummaryContext : DataContractBase
		{
			private readonly WorklistAdminDetail _worklist;
			private readonly bool _isAdmin;

			private bool _hasMultipleWorklists;
			private List<string> _worklistNames;
			private List<string> _worklistDescriptions;
			private List<WorklistClassSummary> _worklistClasses;

			public WorklistSummaryContext(WorklistAdminDetail worklist, bool isAdmin)
			{
				_worklist = worklist;
				_isAdmin = isAdmin;
			}

			[DataMember]
			public bool IsAdmin
			{
				get { return _isAdmin; }
			}

			[DataMember]
			public WorklistAdminDetail Worklist
			{
				get { return _worklist; }
			}

			[DataMember]
			public bool HasMultipleWorklists
			{
				get { return _hasMultipleWorklists; }
				set { _hasMultipleWorklists = value; }
			}

			[DataMember]
			public List<string> WorklistNames
			{
				get { return _worklistNames; }
				set { _worklistNames = value; }
			}

			[DataMember]
			public List<string> WorklistDescriptions
			{
				get { return _worklistDescriptions; }
				set { _worklistDescriptions = value; }
			}

			[DataMember]
			public List<WorklistClassSummary> WorklistClasses
			{
				get { return _worklistClasses; }
				set { _worklistClasses = value; }
			}
		}

		private readonly WorklistSummaryContext _context;

		public WorklistSummaryComponent(WorklistAdminDetail worklist, bool isAdmin)
		{
			_context = new WorklistSummaryContext(worklist, isAdmin);
		}

		public override void Start()
		{
			this.SetUrl(WebResourcesSettings.Default.WorklistSummaryPageUrl);
			base.Start();
		}

		public void SetMultipleWorklistInfo(List<string> names, List<string> descriptions, List<WorklistClassSummary> classes)
		{
			_context.HasMultipleWorklists = true;
			_context.WorklistNames = names;
			_context.WorklistDescriptions = descriptions;
			_context.WorklistClasses = classes;
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}
	}
}
