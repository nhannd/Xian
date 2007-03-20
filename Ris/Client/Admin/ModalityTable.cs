using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    class ModalityTable : Table<ModalitySummary>
    {
        public ModalityTable()
        {
            this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnID,
                delegate(ModalitySummary modality) { return modality.Id; },
                0.2f));

            this.Columns.Add(new TableColumn<ModalitySummary, string>(SR.ColumnName,
                delegate(ModalitySummary modality) { return modality.Name; },
                1.0f));
        }
    }
}
