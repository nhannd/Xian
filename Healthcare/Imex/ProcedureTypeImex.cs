using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("ProcedureType")]
    public class ProcedureTypeImex : XmlEntityImex<ProcedureType, ProcedureTypeImex.ProcedureTypeData>
    {
        [DataContract]
        public class ProcedureTypeData
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
            ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
            where.Id.SortAsc(0);

            return context.GetBroker<IProcedureTypeBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ProcedureTypeData Export(ProcedureType entity, IReadContext context)
        {
            ProcedureTypeData data = new ProcedureTypeData();
            data.Id = entity.Id;
            data.Name = entity.Name;
            if (entity.BaseType != null)
            {
                data.BaseTypeId = entity.BaseType.Id;
            }
            data.PlanXml = entity.GetPlanXml();

            return data;
        }

        protected override void Import(ProcedureTypeData data, IUpdateContext context)
        {
            ProcedureType pt = LoadOrCreateProcedureType(data.Id, data.Name, context);
            if (!string.IsNullOrEmpty(data.BaseTypeId))
            {
                pt.BaseType = GetBaseProcedureType(data.BaseTypeId, context);
            }

            if (data.PlanXml != null)
            {
                pt.SetPlanXml(data.PlanXml);
            }
        }

        #endregion

        private ProcedureType LoadOrCreateProcedureType(string id, string name, IPersistenceContext context)
        {
            ProcedureType pt;
            try
            {
                // see if already exists in db
                ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
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

        private ProcedureType GetBaseProcedureType(string id, IPersistenceContext context)
        {
            ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
            where.Id.EqualTo(id);
            return context.GetBroker<IProcedureTypeBroker>().FindOne(where);
        }
    }
}
