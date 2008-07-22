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
            public string InformationAuthority;
        }

        #region Overrides

        protected override IList<Facility> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            FacilitySearchCriteria where = new FacilitySearchCriteria();
            where.Code.SortAsc(0);
            return context.GetBroker<IFacilityBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override FacilityData Export(Facility entity, IReadContext context)
        {
            FacilityData data = new FacilityData();
			data.Deactivated = entity.Deactivated;
			data.Code = entity.Code;
            data.Name = entity.Name;
            data.InformationAuthority = entity.InformationAuthority.Code;

            return data;
        }

        protected override void Import(FacilityData data, IUpdateContext context)
        {
            InformationAuthorityEnum ia =
                context.GetBroker<IEnumBroker>().Find<InformationAuthorityEnum>(data.InformationAuthority);

            Facility f = LoadOrCreateFacility(data.Code, data.Name, ia, context);
        	f.Deactivated = data.Deactivated;
            f.Name = data.Name;
            f.InformationAuthority = ia;
        }

        #endregion

        private Facility LoadOrCreateFacility(string code, string name, InformationAuthorityEnum ia, IPersistenceContext context)
        {
            Facility pt;
            try
            {
                // see if already exists in db
                FacilitySearchCriteria where = new FacilitySearchCriteria();
                where.Code.EqualTo(code);
                pt = context.GetBroker<IFacilityBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                pt = new Facility(code, name, ia);
                context.Lock(pt, DirtyState.New);
            }

            return pt;
        }
    }
}
