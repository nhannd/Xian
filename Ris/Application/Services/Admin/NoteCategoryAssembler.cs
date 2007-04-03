using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class NoteCategoryAssembler
    {
        public NoteCategoryDetail CreateNoteCategoryDetail(NoteCategory category, IPersistenceContext context)
        {
            if (category == null)
                return null;

            NoteCategoryDetail detail = new NoteCategoryDetail();

            detail.Category = category.Name;
            detail.Description = category.Description;

            NoteSeverityEnumTable severityEnumTable = context.GetBroker<INoteSeverityEnumBroker>().Load();
            detail.Severity = new EnumValueInfo(category.Severity.ToString(), severityEnumTable[category.Severity].Value);

            return detail;
        }

        public NoteCategorySummary CreateNoteCategorySummary(NoteCategory category, IPersistenceContext context)
        {
            if (category == null)
                return null;

            NoteCategorySummary summary = new NoteCategorySummary();

            summary.NoteCategoryRef = category.GetRef();
            summary.Name = category.Name;
            summary.Description = category.Description;

            NoteSeverityEnumTable severityEnumTable = context.GetBroker<INoteSeverityEnumBroker>().Load();
            summary.Severity = new EnumValueInfo(category.Severity.ToString(), severityEnumTable[category.Severity].Value);

            return summary;
        }

        public void UpdateNoteCategory(NoteCategoryDetail detail, NoteCategory category)
        {
            category.Name = detail.Category;
            category.Description = detail.Description;
            category.Severity = (NoteSeverity)Enum.Parse(typeof(NoteSeverity), detail.Severity.Code);
        }
    }
}
