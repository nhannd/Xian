using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerAdminTable : Table<ExternalPractitionerSummary>
	{
		public ExternalPractitionerAdminTable()
		{
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnFamilyName
				, item => item.Name.FamilyName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnGivenName,
				item => item.Name.GivenName, 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, bool>(SR.ColumnVerified,
				item => item.IsVerified, 0.25f));
		}
	}

	public class ExternalPractitionerWorkflowTable : Table<ExternalPractitionerSummary>
	{
		public ExternalPractitionerWorkflowTable()
		{
			DateTimeTableColumn<ExternalPractitionerSummary> timeColumn;

			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnName
				, item => PersonNameFormat.Format(item.Name), 1.0f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
				item => item.LicenseNumber, 0.5f));
			this.Columns.Add(new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
				item => item.BillingNumber, 0.5f));
			this.Columns.Add(timeColumn = new DateTimeTableColumn<ExternalPractitionerSummary>(SR.ColumnTime,
				item => item.LastVerifiedTime, 0.75f));

			this.Sort(new TableSortParams(timeColumn, false));
		}
	}
}
