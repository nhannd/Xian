#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
	[ImexDataClass("Location")]
	public class LocationImex : XmlEntityImex<Location, LocationImex.LocationData>
	{
		[DataContract]
		public class LocationData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string FacilityCode;

			[DataMember]
			public string Building;

			[DataMember]
			public string Floor;

			[DataMember]
			public string PointOfCare;

			[DataMember]
			public string Room;

			[DataMember]
			public string Bed;
		}

		#region Overrides

		protected override IList<Location> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			LocationSearchCriteria where = new LocationSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<ILocationBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override LocationData Export(Location entity, IReadContext context)
		{
			LocationData data = new LocationData();
			data.Deactivated = entity.Deactivated;
			data.Id = entity.Id;
			data.Name = entity.Name;
			data.Description = entity.Description;
			data.FacilityCode = entity.Facility.Code;
			data.Building = entity.Building;
			data.Floor = entity.Floor;
			data.PointOfCare = entity.PointOfCare;
			data.Room = entity.Room;
			data.Bed = entity.Bed;

			return data;
		}

		protected override void Import(LocationData data, IUpdateContext context)
		{
			FacilitySearchCriteria facilityCriteria = new FacilitySearchCriteria();
			facilityCriteria.Code.EqualTo(data.FacilityCode);
			Facility facility = context.GetBroker<IFacilityBroker>().FindOne(facilityCriteria);

			Location l = LoadOrCreateLocation(data.Id, data.Name, facility, context);
			l.Deactivated = data.Deactivated;
			l.Description = data.Description;
			l.Building = data.Building;
			l.Floor = data.Floor;
			l.PointOfCare = data.PointOfCare;
			l.Room = data.Room;
			l.Bed = data.Bed;
		}

		#endregion

		private Location LoadOrCreateLocation(string id, string name, Facility facility, IPersistenceContext context)
		{
			Location l;
			try
			{
				// see if already exists in db
				LocationSearchCriteria where = new LocationSearchCriteria();
				where.Id.EqualTo(id);
				l = context.GetBroker<ILocationBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				l = new Location(id, name, null, facility, null, null, null, null, null);
				context.Lock(l, DirtyState.New);
			}

			return l;
		}
	}
}
