using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services.Admin.RequestedProcedureTypeGroupAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Requested Procedure Type Group Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class RequestedProcedureTypeGroupImporter : DataImporterBase
    {
        private const string tagRoot = "requested-procedure-type-groups";

        private const string tagRequestedProcedureTypeGroup = "requested-procedure-type-group";
        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrCategory = "category";

        private const string tagRequestedProcedureType = "requested-procedure-type";
        
        private const string tagRequestedProcedureTypeRange = "requested-procedure-type-range";
        private const string attrIdMin = "id-min";
        private const string attrIdMax = "id-max";

        private IUpdateContext _context;

        public RequestedProcedureTypeGroupImporter ()
        {

        }
        
        public override bool SupportsXml
        {
            get
            {
                return true;
            }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;

            while (reader.Read())
            {
                if (reader.IsStartElement(tagRoot))
                {
                    ImportRequestedProcedureTypeGroups(reader.ReadSubtree());
                }
            }
        }

        private void ImportRequestedProcedureTypeGroups(XmlReader reader)
        {
            if (reader.ReadToDescendant(tagRequestedProcedureTypeGroup) == false)
            {
                return;
            }
            else
            {
                do 
                {
                    ProcessRequestedProcedureTypeGroupNode(reader);
                } while (reader.ReadToNextSibling(tagRequestedProcedureTypeGroup));
            }
        }

        private void ProcessRequestedProcedureTypeGroupNode(XmlReader reader)
        {
            string name = reader.GetAttribute(attrName);
            string category = reader.GetAttribute(attrCategory);

            RequestedProcedureTypeGroup group = LoadOrCreateRequestedProcedureTypeGroup(name, category);
            if (group != null)
            {
                group.Description = reader.GetAttribute(attrDescription);;
                group.RequestedProcedureTypes.AddAll(GetRequestedProcedureTypes(reader.ReadSubtree()));
            }
        }

        private ICollection GetRequestedProcedureTypes(XmlReader reader)
        {
            List<RequestedProcedureType> types = new List<RequestedProcedureType>();

            for (reader.ReadStartElement(tagRequestedProcedureTypeGroup);
                !(reader.Name == tagRequestedProcedureTypeGroup && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                RequestedProcedureTypeSearchCriteria criteria = new RequestedProcedureTypeSearchCriteria();

                if (reader.IsStartElement(tagRequestedProcedureType))
                {
                    criteria.Name.EqualTo(reader.GetAttribute(attrName));
                }
                else if (reader.IsStartElement(tagRequestedProcedureTypeRange))
                {
                    string idMin = reader.GetAttribute(attrIdMin);
                    string idMax = reader.GetAttribute(attrIdMax);
                    criteria.Id.Between(idMin, idMax);
                }
                else
                {
                    continue;
                }

                types.AddRange(_context.GetBroker<IRequestedProcedureTypeBroker>().Find(criteria));
            }

            return types;
        }

        private RequestedProcedureTypeGroup LoadOrCreateRequestedProcedureTypeGroup(string name, string category)
        {
            RequestedProcedureTypeGroupCategory categoryEnum =
                (RequestedProcedureTypeGroupCategory)Enum.Parse(typeof(RequestedProcedureTypeGroupCategory), category.ToUpper());

            RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
            criteria.Name.EqualTo(name);
            criteria.Category.EqualTo(categoryEnum);

            RequestedProcedureTypeGroup group = null;
            try
            {
                group = _context.GetBroker<IRequestedProcedureTypeGroupBroker>().FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                group = new RequestedProcedureTypeGroup();

                group.Name = name;
                group.Category = categoryEnum;

                _context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
