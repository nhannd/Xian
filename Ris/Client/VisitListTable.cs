using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;
using System.Text;

namespace ClearCanvas.Ris.Client
{
	public class VisitListTable : Table<VisitListItem>
	{
		public VisitListTable()
			: base(2)
		{
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitNumber,
				delegate(VisitListItem _visitListItem) { return VisitNumberFormat.Format(_visitListItem.VisitNumber); },
				1.0f));

			//Visit type description
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitType,
				delegate(VisitListItem _visitListItem)
				{
					return string.Format("{0} - {1} - {2}", 
						_visitListItem.PatientClass.Value, 
						_visitListItem.PatientType.Value, 
						_visitListItem.AdmissionType.Value
					);
				},
				1));

			//status
			this.Columns.Add(new TableColumn<VisitListItem, string>(SR.ColumnVisitStatus,
				delegate(VisitListItem _visitListItem) { return _visitListItem.VisitStatus.Value; },
				1.0f));
			
			//admit date/time
			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnAdmitDateTime,
				delegate(VisitListItem _visitListItem) { return _visitListItem.AdmitTime; },
				1.0f));

			this.Columns.Add(new DateTableColumn<VisitListItem>(SR.ColumnDischargeDateTime,
				delegate(VisitListItem _visitListItem) { return _visitListItem.DischargeTime; },
				1.0f));
		}
	}
}
