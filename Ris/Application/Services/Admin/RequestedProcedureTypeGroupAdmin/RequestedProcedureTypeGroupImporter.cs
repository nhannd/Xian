#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
