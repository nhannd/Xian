using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Modality")]
    public class ModalityImex : XmlEntityImex<Modality, ModalityImex.ModalityData>
    {
        [DataContract]
        public class ModalityData
        {
            [DataMember]
            public string Id;

            [DataMember]
            public string Name;
        }

        #region Overrides

        protected override IList<Modality> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ModalitySearchCriteria where = new ModalitySearchCriteria();
            where.Id.SortAsc(0);

            return context.GetBroker<IModalityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ModalityData Export(Modality entity, IReadContext context)
        {
            ModalityData data = new ModalityData();
            data.Id = entity.Id;
            data.Name = entity.Name;

            return data;
        }

        protected override void Import(ModalityData data, IUpdateContext context)
        {
            Modality m = LoadOrCreateModality(data.Id, data.Name, context);
            m.Name = data.Name;
        }

        #endregion

        private Modality LoadOrCreateModality(string id, string name, IPersistenceContext context)
        {
            Modality pt;
            try
            {
                // see if already exists in db
                ModalitySearchCriteria where = new ModalitySearchCriteria();
                where.Id.EqualTo(id);
                pt = context.GetBroker<IModalityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                pt = new Modality(id, name);
                context.Lock(pt, DirtyState.New);
            }

            return pt;
        }
    }
}
