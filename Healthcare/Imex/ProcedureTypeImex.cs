#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
	[ExtensionOf(typeof(XmlDataImexExtensionPoint))]
	[ImexDataClass("ProcedureType")]
	public class ProcedureTypeImex : XmlEntityImex<ProcedureType, ProcedureTypeImex.ProcedureTypeData>
	{
		[DataContract]
		public class ProcedureTypeData : ReferenceEntityDataBase
		{
			[DataMember]
			public string Id;

			[DataMember]
			public string Name;

			[DataMember]
			public string BaseTypeId;

			[DataMember]
			public XmlDocument PlanXml;
		}



		#region Overrides

		protected override IList<ProcedureType> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
		{
			var where = new ProcedureTypeSearchCriteria();
			where.Id.SortAsc(0);

			return context.GetBroker<IProcedureTypeBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
		}

		protected override ProcedureTypeData Export(ProcedureType entity, IReadContext context)
		{
			var data = new ProcedureTypeData
						{
							Deactivated = entity.Deactivated, 
							Id = entity.Id, 
							Name = entity.Name
						};

			if (entity.BaseType != null)
			{
				data.BaseTypeId = entity.BaseType.Id;
			}
			data.PlanXml = entity.GetPlanXml();

			return data;
		}

		protected override void Import(ProcedureTypeData data, IUpdateContext context)
		{
			var pt = LoadOrCreateProcedureType(data.Id, data.Name, context);
			pt.Deactivated = data.Deactivated;
			if (!string.IsNullOrEmpty(data.BaseTypeId))
			{
				pt.BaseType = GetBaseProcedureType(data.BaseTypeId, context);
			}

			if (data.PlanXml != null)
			{
				pt.SetPlanXml(data.PlanXml);
			}
		}

		/// <summary>
		/// Override the base ImportCore method so we can sort the import item based on the BaseTypeId dependency.
		/// </summary>
		/// <param name="items"></param>
		protected override void ImportCore(IEnumerable<IImportItem> items)
		{
			var sortedProcedureTypeData = new List<ProcedureTypeData>();

			// First sort the list of IImportItem based on the BaseTypeId dependency.
			IEnumerator<IImportItem> enumerator = items.GetEnumerator();
			for (bool more = true; more; )
			{
				for (int j = 0; j < ItemsPerUpdateTransaction && more; j++)
				{
					more = enumerator.MoveNext();
					if (more)
					{
						IImportItem item = enumerator.Current;
						var data = (ProcedureTypeData)Read(item.Read(), typeof(ProcedureTypeData));

						// If there is a dependent data, insert the current data before it, otherwise append it at the end of the list.
						var dependentData = CollectionUtils.SelectFirst(sortedProcedureTypeData, d => d.BaseTypeId != null && d.BaseTypeId == data.Id);
						if (dependentData != null)
						{
							var indexOfDependentData = sortedProcedureTypeData.IndexOf(dependentData);
							sortedProcedureTypeData.Insert(indexOfDependentData, data);
						}
						else
						{
							sortedProcedureTypeData.Add(data);
						}
					}
				}
			}

			// Then import the sorted.
			using (var scope = new PersistenceScope(PersistenceContextType.Update))
			{
				var context = (IUpdateContext)PersistenceScope.CurrentContext;
				context.ChangeSetRecorder.OperationName = this.GetType().FullName;

				foreach (var item in sortedProcedureTypeData)
				{
					Import(item, context);
				}

				scope.Complete();
			}
		}

		#endregion

		private static ProcedureType LoadOrCreateProcedureType(string id, string name, IPersistenceContext context)
		{
			ProcedureType pt;
			try
			{
				// see if already exists in db
				var where = new ProcedureTypeSearchCriteria();
				where.Id.EqualTo(id);
				pt = context.GetBroker<IProcedureTypeBroker>().FindOne(where);
			}
			catch (EntityNotFoundException)
			{
				// create it
				pt = new ProcedureType(id, name);
				context.Lock(pt, DirtyState.New);
			}

			return pt;
		}

		private static ProcedureType GetBaseProcedureType(string id, IPersistenceContext context)
		{
			var where = new ProcedureTypeSearchCriteria();
			where.Id.EqualTo(id);
			return context.GetBroker<IProcedureTypeBroker>().FindOne(where);
		}
	}
}
