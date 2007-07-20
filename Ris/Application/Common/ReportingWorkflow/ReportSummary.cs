using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportSummary : DataContractBase
    {
        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public List<ReportPartSummary> Parts;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string VisitNumberId;

        [DataMember]
        public string VisitNumberAssigningAuthority;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string PerformedLocation;

        [DataMember]
        public DateTime? PerformedDate;

        public ReportPartSummary this[int index]
        {
            get
            {
                if (this.Parts == null)
                    return null;

                return this.Parts[index];
            }
        }

        public string Format()
        {
            return FormatHelper("");
        }

        public string FormatHtml()
        {
            return FormatHelper("<br>");
        }

        private string FormatHelper(string lineBreak)
        {
            StringBuilder builder = new StringBuilder();

            if (this.Parts != null)
            {
                for (int i = this.Parts.Count - 1; i >= 0; i--)
                {
                    ReportPartSummary part = this.Parts[i];

                    if (String.IsNullOrEmpty(part.Content))
                        continue;

                    if (i > 0)
                        builder.Append("Addendum: ");
                    else
                        builder.Append("Report: ");

                    builder.Append(part.Content);
                    builder.AppendLine(lineBreak);
                    builder.AppendLine(lineBreak);
                }
            }

            return builder.ToString();
        }
    }
}
