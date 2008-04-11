using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using Iesi.Collections.Generic;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Procedure Type Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProcedureTypeImporter : DataImporterBase
    {
        private const string tagProcedureTypes = "procedure-types";
        private const string tagProcedureType = "procedure-type";

        private IUpdateContext _context;

        private HashedSet<ProcedureType> _procedureTypes;

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;
            _procedureTypes = new HashedSet<ProcedureType>();

            while (reader.Read())
            {
                if (reader.IsStartElement(tagProcedureTypes))
                {
                    ImportProcedureTypes(reader.ReadSubtree());
                }
            }
        }

        private void ImportProcedureTypes(XmlReader reader)
        {
            for (bool elementExists = reader.ReadToDescendant(tagProcedureType);
                 elementExists;
                 elementExists = reader.ReadToNextSibling(tagProcedureType))
            {
                ProcessProcedureTypeNode(reader.ReadSubtree());
            }
        }

        private void ProcessProcedureTypeNode(XmlReader reader)
        {
            reader.Read();

            string id = reader.GetAttribute("id");
            string name = reader.GetAttribute("name");
            string baseTypeId = reader.GetAttribute("baseType");
            string planXml = reader.ReadInnerXml();

            ProcedureType baseType = string.IsNullOrEmpty(baseTypeId) ? null : GetBaseProcedureType(baseTypeId);
            ProcedureType thisType = GetProcedureType(id, name);
            thisType.BaseType = baseType;

            if(!string.IsNullOrEmpty(planXml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(planXml);

                thisType.SetPlanXml(xmlDoc);
            }

            while (reader.Read());
            reader.Close();
        }

        private ProcedureType GetProcedureType(string id, string name)
        {
            // check if we have it in memory
            ProcedureType pt = CollectionUtils.SelectFirst(_procedureTypes,
                                                           delegate(ProcedureType t)
                                                           {
                                                               return t.Id == id;
                                                           });

            if(pt != null)
                return pt;

            try
            {
                // see if already exists in db
                ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
                where.Id.EqualTo(id);
                pt = _context.GetBroker<IProcedureTypeBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                // create it
                pt = new ProcedureType(id, name);
                _context.Lock(pt, DirtyState.New);
            }

            _procedureTypes.Add(pt);

            return pt;
        }

        private ProcedureType GetBaseProcedureType(string id)
        {
            // check if we have it in memory
            ProcedureType pt = CollectionUtils.SelectFirst(_procedureTypes,
                                                           delegate(ProcedureType t)
                                                           {
                                                               return t.Id == id;
                                                           });

            if (pt != null)
                return pt;

            // get it from DB
            ProcedureTypeSearchCriteria where = new ProcedureTypeSearchCriteria();
            where.Id.EqualTo(id);
            pt = _context.GetBroker<IProcedureTypeBroker>().FindOne(where);

            _procedureTypes.Add(pt);

            return pt;
        }
    }
}