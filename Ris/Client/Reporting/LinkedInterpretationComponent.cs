using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
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


        /// <summary>
        /// Constructor
        /// </summary>
        public LinkedInterpretationComponent(List<ReportingWorklistItem> candidateItems)
        {
            _candidates = candidateItems;
        }

        public override void Start()
        {
            _candidateTable = new Table<Checkable<ReportingWorklistItem>>();
            _candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, bool>(".",
                delegate(Checkable<ReportingWorklistItem> item) { return item.IsChecked; },
                delegate(Checkable<ReportingWorklistItem> item, bool value) { item.IsChecked = value; }, 0.20f));
            _candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnAccessionNumber,
                delegate(Checkable<ReportingWorklistItem> item) { return item.Item.AccessionNumber; }, 0.75f));
            _candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnDiagnosticService,
                delegate(Checkable<ReportingWorklistItem> item) { return item.Item.DiagnosticServiceName; }, 1.0f));
            _candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnProcedure,
                delegate(Checkable<ReportingWorklistItem> item) { return item.Item.RequestedProcedureName; }, 1.0f));
            _candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItem>, string>(SR.ColumnProcedureEndTime,
                delegate(Checkable<ReportingWorklistItem> item) { return Format.Time(item.Item.ScheduledStartTime); }, 0.5f));

            foreach (ReportingWorklistItem item in _candidates)
            {
                _candidateTable.Items.Add(new Checkable<ReportingWorklistItem>(item));
            }

            base.Start();
        }

        public List<ReportingWorklistItem> SelectedItems
        {
            get
            {
                return CollectionUtils.Map<Checkable<ReportingWorklistItem>, ReportingWorklistItem>(
                    CollectionUtils.Select(_candidateTable.Items,
                        delegate(Checkable<ReportingWorklistItem> item) { return item.IsChecked; }),
                            delegate (Checkable<ReportingWorklistItem> checkableItem) { return checkableItem.Item; });
            }
        }

        #region Presentation Model

        public ITable CandidateTable
        {
            get { return _candidateTable; }
        }

        public void Accept()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion
    }
}
