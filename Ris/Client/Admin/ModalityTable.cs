using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    class ModalityTable : Table<Modality>
    {
        public ModalityTable()
        {
            this.Columns.Add(new TableColumn<Modality, string>(SR.ColumnID,
                delegate(Modality modality) { return modality.Id; },
                0.2f));

            this.Columns.Add(new TableColumn<Modality, string>(SR.ColumnName,
                delegate(Modality modality) { return modality.Name; },
                1.0f));
        }
    }
}
