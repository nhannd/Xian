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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("Facility")]
	public class FacilityImex : XmlEntityImex<Facility, FacilityImex.FacilityData>
	{
		[DataContract]
		public class FacilityData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Code;

			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string InformationAuthority;
		}

		#region Overrides

		protected override IList<Facility> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new FacilitySearchCriteria();
			where.Code.SortAsc(0);
			return context.GetBroker<IFacilityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override FacilityData Export(Facility entity, IReadContext context)
		{
			var data = new FacilityData
				{
					Deactivated = entity.Deactivated,
					Code = entity.Code,
					Name = entity.Name,
					Description = entity.Description,
					InformationAuthority = entity.InformationAuthority.Code
				};

			return data;
		}

		protected override void Import(FacilityData data, IUpdateContext context)
		{
			var ia = context.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(data.InformationAuthority);

			var f = LoadOrCreateFacility(data.Code, data.Name, data.Description, ia, context);
			f.Deactivated = data.Deactivated;
			f.Name = data.Name;
			f.InformationAuthority = ia;
		}

		#endregion

		private static Facility LoadOrCreateFacility(string code, string name, string description, InformationAuthorityEnum ia, IPersistenceContext context)
		{
			Facility pt;
			try
			{
				// see if already exists in db
				var where = new FacilitySearchCriteria();
				where.Code.EqualTo(code);
				pt = context.GetBroker<IFacilityBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				pt = new Facility(code, name, description, ia);
				context.Lock(pt, DirtyState.New);
			}

			return pt;
		}
	}
}
