using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("PatientNoteCategory")]
	public class PatientNoteCategoryImex : XmlEntityImex<PatientNoteCategory, PatientNoteCategoryImex.PatientNoteCategoryData>
	{
		[DataContract]
		public class PatientNoteCategoryData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string Severity;
		}

		#region Overrides

		protected override IList<PatientNoteCategory> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			PatientNoteCategorySearchCriteria where = new PatientNoteCategorySearchCriteria();
			where.Name.SortAsc(0);

			return context.GetBroker<IPatientNoteCategoryBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override PatientNoteCategoryData Export(PatientNoteCategory entity, IReadContext context)
		{
			PatientNoteCategoryData data = new PatientNoteCategoryData();
			data.Deactivated = entity.Deactivated;
			data.Name = entity.Name;
			data.Description = entity.Description;
			data.Severity = entity.Severity.ToString();

			return data;
		}

		protected override void Import(PatientNoteCategoryData data, IUpdateContext context)
		{
			NoteSeverity severity = (NoteSeverity) Enum.Parse(typeof (NoteSeverity), data.Severity);
			PatientNoteCategory nc = LoadOrCreatePatientNoteCategory(data.Name, severity, context);
			nc.Deactivated = data.Deactivated;
			nc.Description = data.Description;
		}

		#endregion

		private PatientNoteCategory LoadOrCreatePatientNoteCategory(string name, NoteSeverity severity, IPersistenceContext context)
		{
			PatientNoteCategory nc;
			try
			{
				// see if already exists in db
				PatientNoteCategorySearchCriteria where = new PatientNoteCategorySearchCriteria();
				where.Name.EqualTo(name);
				nc = context.GetBroker<IPatientNoteCategoryBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				nc = new PatientNoteCategory(name, null, severity);
				context.Lock(nc, DirtyState.New);
			}

			return nc;
		}
	}
}
