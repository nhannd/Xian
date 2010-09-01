#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("DiagnosticService")]
	public class DiagnosticServiceImex : XmlEntityImex<DiagnosticService, DiagnosticServiceImex.DiagnosticServiceData>
	{
		[DataContract]
		public class DiagnosticServiceData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public List<ProcedureTypeData> ProcedureTypes;

		}

		[DataContract]
		public class ProcedureTypeData
		{
			[DataMember]
			public string Id;
		}

		#region Overrides

		protected override IList<DiagnosticService> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new DiagnosticServiceSearchCriteria();
			where.Id.SortAsc(0);
			return context.GetBroker<IDiagnosticServiceBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override DiagnosticServiceData Export(DiagnosticService entity, IReadContext context)
		{
			var data = new DiagnosticServiceData
				{
					Deactivated = entity.Deactivated,
					Id = entity.Id,
					Name = entity.Name,
					ProcedureTypes = CollectionUtils.Map<ProcedureType, ProcedureTypeData>(entity.ProcedureTypes, pt => new ProcedureTypeData { Id = pt.Id })
				};

			return data;
		}

		protected override void Import(DiagnosticServiceData data, IUpdateContext context)
		{
			var ds = GetDiagnosticService(data.Id, data.Name, context);
			ds.Deactivated = data.Deactivated;
			ds.Name = data.Name;

			if (data.ProcedureTypes != null)
			{
				foreach (var s in data.ProcedureTypes)
				{
					var where = new ProcedureTypeSearchCriteria();
					where.Id.EqualTo(s.Id);
					var pt = CollectionUtils.FirstElement(context.GetBroker<IProcedureTypeBroker>().Find(where));
					if (pt != null)
						ds.ProcedureTypes.Add(pt);
				}
			}
		}

		#endregion


		private static DiagnosticService GetDiagnosticService(string id, string name, IPersistenceContext context)
		{
			DiagnosticService ds;
			try
			{
				// see if already exists in db
				var where = new DiagnosticServiceSearchCriteria();
				where.Id.EqualTo(id);
				ds = context.GetBroker<IDiagnosticServiceBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				ds = new DiagnosticService(id, name);
				context.Lock(ds, DirtyState.New);
			}

			return ds;
		}
	}
}
