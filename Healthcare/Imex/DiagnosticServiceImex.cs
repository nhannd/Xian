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

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("DiagnosticService")]
    public class DiagnosticServiceImex : XmlEntityImex<DiagnosticService, DiagnosticServiceImex.DiagnosticServiceData>
    {
        [DataContract]
        public class DiagnosticServiceData
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
            DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
            where.Id.SortAsc(0);
            return context.GetBroker<IDiagnosticServiceBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override DiagnosticServiceImex.DiagnosticServiceData Export(DiagnosticService entity, IReadContext context)
        {
            DiagnosticServiceData data = new DiagnosticServiceData();
            data.Id = entity.Id;
            data.Name = entity.Name;
            data.ProcedureTypes = CollectionUtils.Map<ProcedureType,ProcedureTypeData>(
                entity.ProcedureTypes,
                delegate(ProcedureType pt)
                {
                    ProcedureTypeData ptdata = new ProcedureTypeData();
                    ptdata.Id = pt.Id;
                    return ptdata;
                });

            return data;
        }

        protected override void Import(DiagnosticServiceData data, IUpdateContext context)
        {
            DiagnosticService ds = GetDiagnosticService(data.Id, data.Name, context);
            ds.Name = data.Name;

            if (data.ProcedureTypes != null)
            {
                foreach (ProcedureTypeData s in data.ProcedureTypes)
                {
                    ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
                    where.Id.EqualTo(s.Id);
                    ProcedureType pt = CollectionUtils.FirstElement(context.GetBroker<IProcedureTypeBroker>().Find(where));
                    if (pt != null)
                        ds.ProcedureTypes.Add(pt);
                }
            }
        }

        #endregion


        private DiagnosticService GetDiagnosticService(string id, string name, IPersistenceContext context)
        {
            DiagnosticService ds;
            try
            {
                // see if already exists in db
                DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
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
