#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="LinkedInterpretationComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class LinkedInterpretationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// LinkedInterpretationComponent class
	/// </summary>
	[AssociateView(typeof(LinkedInterpretationComponentViewExtensionPoint))]
	public class LinkedInterpretationComponent : ApplicationComponent
	{
		private Table<Checkable<ReportingWorklistItem>> _candidateTable;
		private readonly List<ReportingWorklistItem> _candidates;

		private readonly ReportingWorklistItem _sourceItem;
		private ReportingWorklistTable _sourceTable;

		private readonly string _instructions;
		private readonly string _heading;

		/// <summary>
		/// Constructor
		/// </summary>
		public LinkedInterpretationComponent(ReportingWorklistItem sourceItem, List<ReportingWorklistItem> candidateItems, string instructions, string heading)
		{
			_candidates = candidateItems;
			_sourceItem = sourceItem;
			_instructions = instructions;
			_heading = heading;
		}

		public LinkedInterpretationComponent(ReportingWorklistItem sourceItem, List<ReportingWorklistItem> candidateItems)
			: this(sourceItem, candidateItems, SR.TextLinkReportInstructions, SR.TextLinkReportlHeading)
		{
		}

		public override void Start()
		{
			_candidateTable = new Table<Checkable<ReportingWorklistItem>>();
			_candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, bool>(".",
				delegate(Checkable<ReportingWorklistItem> item) { return item.IsChecked; },
				delegate(Checkable<ReportingWorklistItem> item, bool value) { item.IsChecked = value; }, 0.20f));
			_candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnProcedure,
				delegate(Checkable<ReportingWorklistItem> item) { return item.Item.ProcedureName; }, 2.75f));
			_candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnTime,
				delegate(Checkable<ReportingWorklistItem> item) { return Format.Time(item.Item.Time); }, 0.5f));

			foreach (ReportingWorklistItem item in _candidates)
			{
				_candidateTable.Items.Add(new Checkable<ReportingWorklistItem>(item));
			}

			_sourceTable = new ReportingWorklistTable();
			_sourceTable.Items.Add(_sourceItem);

			base.Start();
		}

		public List<ReportingWorklistItem> SelectedItems
		{
			get
			{
				return CollectionUtils.Map<Checkable<ReportingWorklistItem>, ReportingWorklistItem>(
					CollectionUtils.Select(_candidateTable.Items,
						delegate(Checkable<ReportingWorklistItem> item) { return item.IsChecked; }),
							delegate(Checkable<ReportingWorklistItem> checkableItem) { return checkableItem.Item; });
			}
		}

		#region Presentation Model

		public ITable SourceTable
		{
			get { return _sourceTable; }
		}

		public ITable CandidateTable
		{
			get { return _candidateTable; }
		}

		public string Instructions
		{
			get { return _instructions; }
		}

		public string Heading
		{
			get { return _heading; }
		}

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		#endregion
	}
}
