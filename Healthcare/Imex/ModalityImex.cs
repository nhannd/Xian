#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Modality")]
	public class ModalityImex : XmlEntityImex<Modality, ModalityImex.ModalityData>
	{
		[DataContract]
		public class ModalityData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string FacilityCode;

			[DataMember]
			public string AETitle;

			[DataMember]
			public string DicomModality;
		}

		#region Overrides

		protected override IList<Modality> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new ModalitySearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IModalityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override ModalityData Export(Modality entity, IReadContext context)
		{
			return new ModalityData
				{
					Deactivated = entity.Deactivated,
					Id = entity.Id,
					Name = entity.Name,
					FacilityCode = entity.Facility == null ? null : entity.Facility.Code,
					AETitle = entity.AETitle,
					DicomModality = entity.DicomModality == null ? null : entity.DicomModality.Code
				};
		}

		protected override void Import(ModalityData data, IUpdateContext context)
		{
			var m = LoadOrCreateModality(data.Id, data.Name, context);
			m.Deactivated = data.Deactivated;
			m.Name = data.Name;
			m.Facility = string.IsNullOrEmpty(data.FacilityCode) ? null : FindFacility(data.FacilityCode, context);
			m.AETitle = data.AETitle;
			m.DicomModality = data.DicomModality == null ? null : context.GetBroker<IEnumBroker>().Find<DicomModalityEnum>(data.DicomModality);
		}

		#endregion

		private static Modality LoadOrCreateModality(string id, string name, IPersistenceContext context)
		{
			Modality pt;
			try
			{
				// see if already exists in db
				var where = new ModalitySearchCriteria();
				where.Id.EqualTo(id);
				pt = context.GetBroker<IModalityBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				pt = new Modality(id, name, null, null, null);
				context.Lock(pt, DirtyState.New);
			}

			return pt;
		}

		private Facility FindFacility(string facilityCode, IPersistenceContext context)
		{
			var facilityCriteria = new FacilitySearchCriteria();
			facilityCriteria.Code.EqualTo(facilityCode);
			return context.GetBroker<IFacilityBroker>().FindOne(facilityCriteria);
		}

	}
}
