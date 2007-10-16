using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class ProcedurePlanSummaryTableItem
    {
        private readonly RequestedProcedureDetail _rpDetail;
        private readonly ModalityProcedureStepDetail _mpsDetail;

        public ProcedurePlanSummaryTableItem(RequestedProcedureDetail rpDetail, ModalityProcedureStepDetail mpsDetail)
        {
            _rpDetail = rpDetail;
            _mpsDetail = mpsDetail;
        }

        #region Public Properties

        public RequestedProcedureDetail rpDetail
        {
            get { return _rpDetail; }
        }

        public ModalityProcedureStepDetail mpsDetail
        {
            get { return _mpsDetail; }
        }

        #endregion
    }

    public class ProcedurePlanSummaryTable : DecoratedTable<Checkable<ProcedurePlanSummaryTableItem>>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint ProcedureDescriptionRow = 1;

        private event EventHandler _checkedRowsChanged;

        public ProcedurePlanSummaryTable()
            : this(NumRows)
        {
        }

        private ProcedurePlanSummaryTable(uint cellRowCount)
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
                "Status",
                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.mpsDetail.Status.Value; },
                0.5f));

            this.Columns.Add(new TableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>(
                "Modality",
                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable) { return checkable.Item.mpsDetail.ModalityName; },
                0.5f));

            ITableColumn sortColumn = new DecoratedTableColumn<Checkable<ProcedurePlanSummaryTableItem>, string>("Procedure Description",
                delegate(Checkable<ProcedurePlanSummaryTableItem> checkable)
                {
                    return string.Format("{0} - {1}", checkable.Item.rpDetail.Name, checkable.Item.mpsDetail.Name);
                },
                0.5f,
                ProcedureDescriptionRow);
            this.Columns.Add(sortColumn);

            this.Sort(new TableSortParams(sortColumn, true));
        }

        public event EventHandler CheckedRowsChanged
        {
            add { _checkedRowsChanged += value; }
            remove { _checkedRowsChanged -= value; }
        }
    }
}