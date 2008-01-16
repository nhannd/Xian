#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin.ProcedureTypeGroupAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Procedure Type Group Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ProcedureTypeGroupImporter : DataImporterBase
    {
        private const string tagRoot = "procedure-type-groups";

        private const string tagProcedureTypeGroup = "procedure-type-group";
        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrClass = "class";

        private const string tagProcedureType = "procedure-type";
        
        private const string tagProcedureTypeRange = "procedure-type-range";
        private const string attrIdMin = "id-min";
        private const string attrIdMax = "id-max";

        private IUpdateContext _context;
        private IList<Type> _procedureTypeGroupClasses;

        public ProcedureTypeGroupImporter ()
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

            if(_procedureTypeGroupClasses == null)
            {
                _procedureTypeGroupClasses = ProcedureTypeGroup.ListSubClasses(context);
            }

            while (reader.Read())
            {
                if (reader.IsStartElement(tagRoot))
                {
                    ImportProcedureTypeGroups(reader.ReadSubtree());
                }
            }
        }

        private void ImportProcedureTypeGroups(XmlReader reader)
        {
            if (reader.ReadToDescendant(tagProcedureTypeGroup) == false)
            {
                return;
            }
            else
            {
                do 
                {
                    ProcessProcedureTypeGroupNode(reader);
                } while (reader.ReadToNextSibling(tagProcedureTypeGroup));
            }
        }

        private void ProcessProcedureTypeGroupNode(XmlReader reader)
        {
            string name = reader.GetAttribute(attrName);
            string category = reader.GetAttribute(attrClass);

            ProcedureTypeGroup group = LoadOrCreateProcedureTypeGroup(name, category);
            if (group != null)
            {
                group.Description = reader.GetAttribute(attrDescription);;
                group.ProcedureTypes.AddAll(GetProcedureTypes(reader.ReadSubtree()));
            }
        }

        private ICollection<ProcedureType> GetProcedureTypes(XmlReader reader)
        {
            List<ProcedureType> types = new List<ProcedureType>();

            for (reader.ReadStartElement(tagProcedureTypeGroup);
                !(reader.Name == tagProcedureTypeGroup && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                ProcedureTypeSearchCriteria criteria = new ProcedureTypeSearchCriteria();

                if (reader.IsStartElement(tagProcedureType))
                {
                    criteria.Name.EqualTo(reader.GetAttribute(attrName));
                }
                else if (reader.IsStartElement(tagProcedureTypeRange))
                {
                    string idMin = reader.GetAttribute(attrIdMin);
                    string idMax = reader.GetAttribute(attrIdMax);
                    criteria.Id.Between(idMin, idMax);
                }
                else
                {
                    continue;
                }

                types.AddRange(_context.GetBroker<IProcedureTypeBroker>().Find(criteria));
            }

            return types;
        }

        private ProcedureTypeGroup LoadOrCreateProcedureTypeGroup(string name, string category)
        {
            Type groupClass = CollectionUtils.SelectFirst(_procedureTypeGroupClasses,
                delegate(Type t)
                {
                    return t.FullName.Equals(category, StringComparison.InvariantCultureIgnoreCase);
                });

            ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
            criteria.Name.EqualTo(name);

            ProcedureTypeGroup group = null;
            try
            {
                group = _context.GetBroker<IProcedureTypeGroupBroker>().FindOne(criteria, groupClass);
            }
            catch (EntityNotFoundException)
            {
                group = (ProcedureTypeGroup) Activator.CreateInstance(groupClass);
                group.Name = name;

                _context.Lock(group, DirtyState.New);
            }

            return group;
        }
    }
}
