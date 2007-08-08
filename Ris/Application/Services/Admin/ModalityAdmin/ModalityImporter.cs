using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.ModalityAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Modality Importer")]
    public class ModalityImporter : DataImporterBase
    {
        public ModalityImporter()
        {

        }

        public override bool SupportsCsv
        {
            get { return true; }
        }

        public override void ImportCsv(List<string> rows, IUpdateContext context)
        {
            List<Modality> modalities = new List<Modality>();
        
            foreach (string line in rows)
            {
                // expect 2 fields in the row
                string[] fields = ParseCsv(line, 2);

                string id = fields[0];
                string name = fields[1];

                // first check if we have it in memory
                Modality modality = CollectionUtils.SelectFirst<Modality>(modalities,
                    delegate(Modality sp) { return sp.Id == id && sp.Name == name; });

                // if not, check the database
                if (modality == null)
                {
                    ModalitySearchCriteria where = new ModalitySearchCriteria();
                    where.Id.EqualTo(id);
                    where.Name.EqualTo(name);

                    IModalityBroker broker = context.GetBroker<IModalityBroker>();
                    modality = CollectionUtils.FirstElement<Modality>(broker.Find(where));

                    // if not, create a new instance
                    if (modality == null)
                    {
                        modality = new Modality(id, name);
                        context.Lock(modality, DirtyState.New);
                    }

                    modalities.Add(modality);
                }
            }
        }
    }
}
