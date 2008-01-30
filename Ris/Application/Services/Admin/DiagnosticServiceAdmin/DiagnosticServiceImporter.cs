using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DiagnosticServiceImporter : DataImporterBase
    {
        private const string tagDiagnosticServices = "diagnostic-services";
        private const string tagDiagnosticService = "diagnostic-service";
        private const string tagProcedureTypes = "procedure-types";
        private const string tagProcedureType = "procedure-type";

        private IUpdateContext _context;

        private HashedSet<DiagnosticService> _diagnosticServices;
        private HashedSet<ProcedureType> _procedureTypes;

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;
            _diagnosticServices = new HashedSet<DiagnosticService>();
            _procedureTypes = new HashedSet<ProcedureType>();

            while (reader.Read())
            {
                if (reader.IsStartElement(tagDiagnosticServices))
                {
                    ReadDiagnosticServices(reader.ReadSubtree());
                }
            }
        }

        private void ReadDiagnosticServices(XmlReader reader)
        {
            for (bool elementExists = reader.ReadToDescendant(tagDiagnosticService);
                 elementExists;
                 elementExists = reader.ReadToNextSibling(tagDiagnosticService))
            {
                ReadDiagnosticServiceNode(reader.ReadSubtree());
            }
        }

        private void ReadDiagnosticServiceNode(XmlReader reader)
        {
            reader.Read();

            string id = reader.GetAttribute("id");
            string name = reader.GetAttribute("name");

            // get the diagnostic service (or create it)
            DiagnosticService ds = GetDiagnosticService(id, name);

            reader.ReadToFollowing(tagProcedureTypes);
            ICollection<ProcedureType> procedureTypes = ReadProcedureTypes(reader.ReadSubtree());
            ds.ProcedureTypes.AddAll(procedureTypes);

            while (reader.Read()) ;
            reader.Close();
        }

        private ICollection<ProcedureType> ReadProcedureTypes(XmlReader reader)
        {
            reader.Read();

            List<ProcedureType> types = new List<ProcedureType>();

            for (bool elementExists = reader.ReadToDescendant(tagProcedureType);
                elementExists;
                elementExists = reader.ReadToNextSibling(tagProcedureType))
            {
                string id = reader.GetAttribute("id");
                types.Add(GetProcedureType(id));
            }

            while (reader.Read()) ;
            reader.Close();

            return types;
        }
        
        private DiagnosticService GetDiagnosticService(string id, string name)
        {
            // check if we have it in memory
            DiagnosticService ds = CollectionUtils.SelectFirst(_diagnosticServices,
                                                           delegate(DiagnosticService s)
                                                           {
                                                               return s.Id == id;
                                                           });

            if (ds != null)
                return ds;

            try
            {
                // see if already exists in db
                DiagnosticServiceSearchCriteria where = new DiagnosticServiceSearchCriteria();
                where.Id.EqualTo(id);
                ds = _context.GetBroker<IDiagnosticServiceBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                ds = new DiagnosticService(id, name);
                _context.Lock(ds, DirtyState.New);
            }

            _diagnosticServices.Add(ds);

            return ds;
        }

        private ProcedureType GetProcedureType(string id)
        {
            // check if we have it in memory
            ProcedureType pt = CollectionUtils.SelectFirst(_procedureTypes,
                                                           delegate(ProcedureType t)
                                                           {
                                                               return t.Id == id;
                                                           });

            if (pt != null)
                return pt;

            // get procedure type from db
            // if doesn't exist, throw an exception

            ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
            where.Id.EqualTo(id);
            pt = _context.GetBroker<IProcedureTypeBroker>().FindOne(where);

            _procedureTypes.Add(pt);

            return pt;
        }
    }
}
