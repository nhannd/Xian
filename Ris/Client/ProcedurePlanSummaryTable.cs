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

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class ProcedurePlanSummaryTableItem
    {
        private readonly ProcedureDetail _rpDetail;
        private readonly ProcedureStepDetail _mpsDetail;

        public ProcedurePlanSummaryTableItem(ProcedureDetail rpDetail, ProcedureStepDetail mpsDetail)
        {
            _rpDetail = rpDetail;
            _mpsDetail = mpsDetail;
        }

        #region Public Properties

        public ProcedureDetail rpDetail
        {
            get { return _rpDetail; }
        }

        public ProcedureStepDetail mpsDetail
        {
            get { return _mpsDetail; }
        }

        #endregion
    }

    public class ProcedurePlanSummaryTable : Table<Checkable<ProcedurePlanSummaryTableItem>>
    {
        private static readonly int NumRows = 2;
        private static readonly int ProcedureDescriptionRow = 1;

        private event EventHandler _checkedRowsChanged;

        public ProcedurePlanSummaryTable()
            : this(NumRows)
        {
        }

        private ProcedurePlanSummaryTable(int cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, bool>(
                                 "X",
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.IsChecked; },
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable, bool isChecked)
                                     {
                                         checkable.IsChecked = isChecked;
                                         EventsHelper.Fire(_checkedRowsChanged, this, EventArgs.Empty);
                                     },
                                 0.1f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 SR.ColumnStatus,
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                                 {
                                 	return FormatStatus(checkable.Item);
                                 },
                                 0.5f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                                 SR.ColumnModality,
                                 delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.mpsDetail.Modality.Name; },
                                 0.5f));

			TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string> sortColumn = 
                new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
				     SR.ColumnScheduledStartTime,
				     delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return Format.DateTime(checkable.Item.mpsDetail.ScheduledStartTime); },
				     0.5f);

			sortColumn.Comparison = delegate(Checkable<ProcedurePlanSummaryTableItem> x, Checkable<ProcedurePlanSummaryTableItem> y)
                { return Nullable.Compare(x.Item.mpsDetail.ScheduledStartTime, y.Item.mpsDetail.ScheduledStartTime); };

			this.Columns.Add(sortColumn);

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(SR.ColumnProcedureDescription,
                                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                                    {
                                        return string.Format("{0} - {1}", 
                                            checkable.Item.mpsDetail.ProcedureStepName, 
                                            checkable.Item.rpDetail.CheckInTime == null 
                                                ? SR.FormatNotCheckedIn
                                                : string.Format(SR.FormatCheckedInTime, Format.DateTime(checkable.Item.rpDetail.CheckInTime)));
                                    },
                                0.5f,
                                ProcedureDescriptionRow));

            this.Sort(new TableSortParams(sortColumn, true));
        }

    	private static string FormatStatus(ProcedurePlanSummaryTableItem item)
    	{
			if(item.mpsDetail.State.Code == "SC")
				return item.rpDetail.CheckInTime.HasValue
                    ? string.Format(SR.FormatStatusCheckedIn, item.mpsDetail.State.Value)
					: item.mpsDetail.State.Value;
			else
	    		return item.mpsDetail.State.Value;
    	}

    	public event EventHandler CheckedRowsChanged
        {
            add { _checkedRowsChanged += value; }
            remove { _checkedRowsChanged -= value; }
        }
    }
}