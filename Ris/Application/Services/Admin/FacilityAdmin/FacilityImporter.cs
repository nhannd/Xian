using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Facility Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class FacilityImporter : DataImporterBase
    {
        public FacilityImporter()
        {

        }

        public override bool SupportsCsv
        {
            get { return true; }
        }

        public override void ImportCsv(List<string> rows, IUpdateContext context)
        {
           List<Facility> facilities = new List<Facility>();

           foreach (string line in rows)
            {
                // expect 2 fields in the row
                string[] fields = ParseCsv(line, 2);

                string id = fields[0];
                string name = fields[1];

                // first check if we have it in memory
                Facility facility = CollectionUtils.SelectFirst<Facility>(facilities,
                    delegate(Facility f) { return f.Code == id && f.Name == name; });

                // if not, check the database
                if (facility == null)
                {
                    FacilitySearchCriteria where = new FacilitySearchCriteria();
                    where.Code.EqualTo(id);
                    where.Name.EqualTo(name);

                    IFacilityBroker broker = context.GetBroker<IFacilityBroker>();
                    facility = CollectionUtils.FirstElement<Facility>(broker.Find(where));

                    // if not, create a new instance
                    if (facility == null)
                    {
                        facility = new Facility(id, name);
                        context.Lock(facility, DirtyState.New);
                    }

                    facilities.Add(facility);
                }
            }
        }
    }
}
