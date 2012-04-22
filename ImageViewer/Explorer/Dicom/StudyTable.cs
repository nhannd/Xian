using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class StudyTable : Table<StudyTableItem>
    {
        public const string ColumnNamePatientId = @"Patient ID";
        public const string ColumnNameLastName = @"Last Name";
        public const string ColumnNameFirstName = @"First Name";
        public const string ColumnNameIdeographicName = @"Ideographic Name";
        public const string ColumnNamePhoneticName = @"Phonetic Name";
        public const string ColumnNameDateOfBirth = @"DOB";
        public const string ColumnNameAccessionNumber = @"Accession Number";
        public const string ColumnNameStudyDate = @"Study Date";
        public const string ColumnNameStudyDescription = @"Study Description";
        public const string ColumnNameModality = @"Modality";
        public const string ColumnNameAttachments = @"Attachments";
        public const string ColumnNameReferringPhysician = @"Referring Physician";
        public const string ColumnNameDeleteOn = @"Delete On";
        public const string ColumnNameNumberOfInstances = @"Instances";
        public const string ColumnNameServer = @"Server";
        public const string ColumnNameAvailability = @"Availability";

        private TableColumn<StudyTableItem, string> ColumnPatientId { get; set; }
        private TableColumn<StudyTableItem, string> ColumnLastName { get; set; }
        private TableColumn<StudyTableItem, string> ColumnFirstName { get; set; }
        private TableColumn<StudyTableItem, string> ColumnIdeographicName { get; set; }
        private TableColumn<StudyTableItem, string> ColumnPhoneticName { get; set; }
        private TableColumn<StudyTableItem, string> ColumnDateOfBirth { get; set; }
        private TableColumn<StudyTableItem, string> ColumnAccessionNumber { get; set; }
        private TableColumn<StudyTableItem, string> ColumnStudyDate { get; set; }
        private TableColumn<StudyTableItem, string> ColumnStudyDescription { get; set; }
        private TableColumn<StudyTableItem, string> ColumnModality { get; set; }
        private TableColumn<StudyTableItem, IconSet> ColumnAttachments { get; set; }
        private TableColumn<StudyTableItem, string> ColumnReferringPhysician { get; set; }
        private TableColumn<StudyTableItem, string> ColumnDeleteOn { get; set; }
        private TableColumn<StudyTableItem, string> ColumnNumberOfInstances { get; set; }
        private TableColumn<StudyTableItem, string> ColumnServer { get; set; }
        private TableColumn<StudyTableItem, string> ColumnAvailability { get; set; }

        public void Initialize()
        {
            AddExtensionColumns();
            AddDefaultColumns();
            AddInstanceCountColumns();
            AddServerColumns();

            Sort(new TableSortParams(ColumnLastName, true));
        }

        public void SetServerColumnsVisibility(bool visible)
        {
            ColumnServer.Visible = ColumnAvailability.Visible = visible;
        }

        public void SetIdeographicAndPhoneticNamesVisibility(bool visible)
        {
            ColumnIdeographicName.Visible = ColumnPhoneticName.Visible = visible;
        }

        public void SetColumnVisibility(string columnHeading, bool visible)
        {
            var column = FindColumn(columnHeading);
            if (column == null)
                return;

            column.Visible = visible;
        }

        private void AddDefaultColumns()
        {
            ColumnPatientId = new TableColumn<StudyTableItem, string>(
                ColumnNamePatientId,
                SR.ColumnHeadingPatientId,
                item => item.PatientId,
                0.5f);

            Columns.Add(ColumnPatientId);

            ColumnLastName = new TableColumn<StudyTableItem, string>(
                ColumnNameLastName,
                SR.ColumnHeadingLastName,
                item => new PersonName(item.PatientsName).LastName,
                0.5f);

            Columns.Add(ColumnLastName);

            ColumnFirstName = new TableColumn<StudyTableItem, string>(
                ColumnNameFirstName,
                SR.ColumnHeadingFirstName,
                item => new PersonName(item.PatientsName).FirstName,
                0.5f);

            Columns.Add(ColumnFirstName);

            ColumnIdeographicName = new TableColumn<StudyTableItem, string>(
                ColumnNameIdeographicName,
                SR.ColumnHeadingIdeographicName,
                item => new PersonName(item.PatientsName).Ideographic,
                0.5f) { Visible = false };

            Columns.Add(ColumnIdeographicName);

            ColumnPhoneticName = new TableColumn<StudyTableItem, string>(
                ColumnNamePhoneticName,
                SR.ColumnHeadingPhoneticName,
                item => new PersonName(item.PatientsName).Phonetic,
                0.5f) { Visible = false };

            Columns.Add(ColumnPhoneticName);

            ColumnDateOfBirth = new TableColumn<StudyTableItem, string>(
                ColumnNameDateOfBirth,
                SR.ColumnHeadingDateOfBirth,
                item => FormatDicomDA(item.PatientsBirthDate),
                null,
                0.4F,
                //TODO (Marmot):
                (one, two) => one.PatientsBirthDate.CompareTo(two.PatientsBirthDate));

            Columns.Add(ColumnDateOfBirth);

            ColumnAccessionNumber = new TableColumn<StudyTableItem, string>(
                ColumnNameAccessionNumber,
                SR.ColumnHeadingAccessionNumber,
                item => item.AccessionNumber,
                0.45F);

            Columns.Add(ColumnAccessionNumber);

            ColumnStudyDate = new TableColumn<StudyTableItem, string>(
                ColumnNameStudyDate,
                SR.ColumnHeadingStudyDate,
                item => FormatDicomDA(item.StudyDate),
                null,
                0.4F,
                (one, two) => one.StudyDate.CompareTo(two.StudyDate));

            Columns.Add(ColumnStudyDate);

            ColumnStudyDescription = new TableColumn<StudyTableItem, string>(
                ColumnNameStudyDescription,
                SR.ColumnHeadingStudyDescription,
                item => item.StudyDescription,
                0.75F);

            Columns.Add(ColumnStudyDescription);

            ColumnModality = new TableColumn<StudyTableItem, string>(
                ColumnNameModality,
                SR.ColumnHeadingModality,
                item => DicomStringHelper.GetDicomStringArray(SortModalities(item.ModalitiesInStudy)),
                0.25f);

            Columns.Add(ColumnModality);

            ColumnAttachments = new TableColumn<StudyTableItem, IconSet>(
                ColumnNameAttachments,
                SR.ColumnHeadingAttachments,
                GetAttachmentsIcon,
                0.25f)
                                 {
                                     ResourceResolver = new ApplicationThemeResourceResolver(typeof(SearchResult).Assembly),
                                     Comparison = (x, y) => x.HasAttachments().CompareTo(y.HasAttachments())
                                 };

            Columns.Add(ColumnAttachments);

            ColumnReferringPhysician = new TableColumn<StudyTableItem, string>(
                ColumnNameReferringPhysician,
                SR.ColumnHeadingReferringPhysician,
                delegate(StudyTableItem entry)
                    {
                        var name = new PersonName(entry.ReferringPhysiciansName ?? "");
                        return name.FormattedName;
                    },
                0.6f);

            Columns.Add(ColumnReferringPhysician);

            ColumnDeleteOn = new TableColumn<StudyTableItem, string>(
                ColumnNameDeleteOn,
                SR.ColumnHeadingDeleteOn,
                entry => FormatDeleteOn(entry.DeleteTime),
                0.2f);

            Columns.Add(ColumnDeleteOn);
        }

        private void AddInstanceCountColumns()
        {
            ColumnNumberOfInstances = new TableColumn<StudyTableItem, string>(
                ColumnNameNumberOfInstances,
                SR.ColumnHeadingNumberOfInstances,
                item => item.NumberOfStudyRelatedInstances.HasValue ? item.NumberOfStudyRelatedInstances.ToString() : "",
                null,
                0.3f,
                delegate(StudyTableItem entry1, StudyTableItem entry2)
                    {
                        int? instances1 = entry1.NumberOfStudyRelatedInstances;
                        int? instances2 = entry2.NumberOfStudyRelatedInstances;

                        if (instances1 == null)
                        {
                            if (instances2 == null)
                                return 0;
                            return 1;
                        }
                        if (instances2 == null)
                        {
                            return -1;
                        }

                        return -instances1.Value.CompareTo(instances2.Value);
                    });

            Columns.Add(ColumnNumberOfInstances);
        }

        private void AddServerColumns()
        {
            ColumnServer = new TableColumn<StudyTableItem, string>(ColumnNameServer, SR.ColumnHeadingServer,
                                                                 item => (item.Server == null) ? "" : item.Server.ToString(),
                                                                 0.3f);

            Columns.Add(ColumnServer);

            ColumnAvailability = new TableColumn<StudyTableItem, string>(ColumnNameAvailability, SR.ColumnHeadingAvailability,
                                                             item => item.InstanceAvailability ?? "",
                                                             0.3f);

            Columns.Add(ColumnAvailability);
        }

        private void AddExtensionColumns()
        {
            try
            {
                // Create and add any extension columns
                var xp = new StudyColumnExtensionPoint();
                foreach (IStudyColumn extensionColumn in xp.CreateExtensions())
                {
                    IStudyColumn newColumn = extensionColumn;

                    var column = new TableColumn<StudyTableItem, string>(
                        newColumn.Name,
                        item => (newColumn.GetValue(item) ?? "").ToString(),
                        newColumn.WidthFactor);

                    newColumn.ColumnValueChanged += OnColumnValueChanged;
                    Columns.Add(column);
                }
            }
            catch (NotSupportedException) { }
        }

        public TableColumnBase<StudyTableItem> FindColumn(string columnHeading)
        {
            return Columns.FirstOrDefault(column => column.Name == columnHeading);
        }

        private void OnColumnValueChanged(object sender, ItemEventArgs<StudyTableItem> e)
        {
            Items.NotifyItemUpdated(e.Item);
        }

        private static string FormatDeleteOn(DateTime? deleteOn)
        {
            if (!deleteOn.HasValue)
                return String.Empty;

            if (deleteOn.Value > DateTime.Now)
                return SR.PastDue;

            if (deleteOn.Value.Date == DateTime.Now.Date)
                return String.Format(SR.FormatDueToday, deleteOn.Value.ToString("H:mm"));

            if (deleteOn.Value.Date == DateTime.Now.AddDays(1).Date)
                return String.Format(SR.FormatDueTomorrow, deleteOn.Value.ToString("H:mm"));

            //More than 5 days away, only show the date.
            if (deleteOn.Value.Date > DateTime.Now.AddDays(5).Date)
                return deleteOn.Value.ToString(Format.DateFormat);

            return deleteOn.Value.ToString(Format.DateTimeFormat);
        }

        private static string FormatDicomDA(string dicomDate)
		{
			DateTime date;
			if (!DateParser.Parse(dicomDate, out date))
				return dicomDate;

			return date.ToString(Format.DateFormat);
		}

		private static IconSet GetAttachmentsIcon(StudyTableItem entry)
		{
			return entry.HasAttachments() ? new IconSet("AttachmentsExtraSmall.png") : null;
		}

		private static string[] SortModalities(IEnumerable<string> modalities)
		{
			var list = new List<string>(modalities);
			list.Remove(@"DOC"); // the DOC modality is a special case and handled via the attachments icon
			list.Sort((x, y) =>
			              {
			                  var result = GetModalityPriority(x).CompareTo(GetModalityPriority(y));
			                  if (result == 0)
			                      result = string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
			                  return result;
			              });
			return list.ToArray();
		}

		private static int GetModalityPriority(string modality)
		{
			const int imageModality = 0; // sort all known image modalities to top
			const int unknownModality = 1; // unknown modalities may be images or may simply be other documents - sort after known images, but before known ancillary documents
			const int srModality = 2;
			const int koModality = 3;
			const int prModality = 4;

			switch (modality)
			{
				case @"SR":
					return srModality;
				case @"KO":
					return koModality;
				case @"PR":
					return prModality;
				default:
					return StandardModalities.Modalities.Contains(modality) ? imageModality : unknownModality;
			}
		}
    }
}
