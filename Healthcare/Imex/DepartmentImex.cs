#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	[ImexDataClass("Department")]
	public class DepartmentImex : XmlEntityImex<Department, DepartmentImex.DepartmentData>
	{
		[DataContract]
		public class DepartmentData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string Description;

			[DataMember]
			public string FacilityCode;
		}

		#region Overrides

		protected override IList<Department> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new DepartmentSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IDepartmentBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override DepartmentData Export(Department entity, IReadContext context)
		{
			var data = new DepartmentData
			           	{
			           		Deactivated = entity.Deactivated,
			           		Id = entity.Id,
			           		Name = entity.Name,
			           		Description = entity.Description,
			           		FacilityCode = entity.Facility.Code
			           	};

			return data;
		}

		protected override void Import(DepartmentData data, IUpdateContext context)
		{
			var facilityCriteria = new FacilitySearchCriteria();
			facilityCriteria.Code.EqualTo(data.FacilityCode);
			var facility = context.GetBroker<IFacilityBroker>().FindOne(facilityCriteria);

			var d = LoadOrCreateDepartment(data.Id, data.Name, facility, context);
			d.Deactivated = data.Deactivated;
			d.Description = data.Description;
		}

		#endregion

		private static Department LoadOrCreateDepartment(string id, string name, Facility facility, IPersistenceContext context)
		{
			Department d;
			try
			{
				// see if already exists in db
				var where = new DepartmentSearchCriteria();
				where.Id.EqualTo(id);
				d = context.GetBroker<IDepartmentBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				d = new Department(id, name, facility, null);
				context.Lock(d, DirtyState.New);
			}

			return d;
		}
	}
}
