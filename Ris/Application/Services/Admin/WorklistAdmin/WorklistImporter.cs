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
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ExtensionOf(typeof(DataImporterExtensionPoint), Name = "Worklist Importer")]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class WorklistImporter : DataImporterBase
    {
        private const string tagRoot = "worklists";

        private const string tagWorklist = "worklist";
        private const string attrName = "name";
        private const string attrDescription = "description";
        private const string attrDiscriminator = "discriminator";

        private const string tagProcedureTypeGroups = "procedure-type-groups";
        private const string tagProcedureTypeGroup = "procedure-type-group";
        private const string attrGroupName = "name";
        private const string attrGroupClass = "class";

        private const string tagSubscribers = "subscribers";
        private const string tagSubscriber = "subscriber";
        private const string attrSubscriberName = "name";
        private const string attrSubscriberType = "type";

        private const string tagWorklistCopies = "worklist-copies";
        private const string tagCopy = "copy";
        private const string attrCopyDiscriminator = "discriminator";


        private IUpdateContext _context;
        private IList<Type> _procedureTypeGroupClasses;

        #region DateImporterBase overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;
            if (_procedureTypeGroupClasses == null)
            {
                _procedureTypeGroupClasses = ProcedureTypeGroup.ListSubClasses(context);
            }

            while (reader.Read())
            {
                if (reader.IsStartElement(tagRoot))
                {
                    ImportWorklists(reader.ReadSubtree());
                }
            }
        }

        #endregion

        private void ImportWorklists(XmlReader xmlReader)
        {
            for (xmlReader.ReadToDescendant(tagWorklist);
                !(xmlReader.Name == tagRoot && xmlReader.NodeType == XmlNodeType.EndElement);
                xmlReader.ReadToNextSibling(tagWorklist))
            {
                if (xmlReader.IsStartElement(tagWorklist))
                {
                    ProcessWorklistNode(xmlReader);
                }
            }
        }

        private void ProcessWorklistNode(XmlReader reader)
        {
            string name = reader.GetAttribute(attrName);
            string discriminator = reader.GetAttribute(attrDiscriminator);

            Worklist worklist = LoadOrCreateWorklist(name, discriminator);
            if (worklist != null)
            {
                string description = reader.GetAttribute(attrDescription);

                reader.ReadToFollowing(tagProcedureTypeGroups);
                ICollection<ProcedureTypeGroup> procedureTypeGroups = GetProcedureTypeGroups(reader.ReadSubtree());

                reader.ReadToFollowing(tagSubscribers);
                ICollection<User> users = GetUsers(reader.ReadSubtree());

                worklist.Description = description;
                worklist.ProcedureTypeGroups.AddAll(procedureTypeGroups);
                worklist.Users.AddAll(users);

                reader.ReadToFollowing(tagWorklistCopies);
                foreach (Worklist copy in GetWorklistCopies(reader.ReadSubtree(), name))
                {
                    copy.Description = description;
                    copy.ProcedureTypeGroups.AddAll(procedureTypeGroups);
                    copy.Users.AddAll(users);
                }
            }
        }

        private ICollection<ProcedureTypeGroup> GetProcedureTypeGroups(XmlReader reader)
        {
            List<ProcedureTypeGroup> groups = new List<ProcedureTypeGroup>();

            for (reader.ReadToDescendant(tagProcedureTypeGroups);
                !(reader.Name == tagProcedureTypeGroups && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                if (reader.IsStartElement(tagProcedureTypeGroup))
                {
                    ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
                    criteria.Name.EqualTo(reader.GetAttribute(attrGroupName));

                    string groupClassName = reader.GetAttribute(attrGroupClass);
                    Type groupClass = CollectionUtils.SelectFirst(_procedureTypeGroupClasses,
                       delegate(Type t)
                       {
                           return t.FullName.Equals(groupClassName, StringComparison.InvariantCultureIgnoreCase);
                       });

                    IProcedureTypeGroupBroker broker = _context.GetBroker<IProcedureTypeGroupBroker>();
                    ProcedureTypeGroup group = CollectionUtils.FirstElement(broker.Find(criteria, groupClass));
                    if (group != null) groups.Add(group);
                }
            }

            return groups;
        }

        private ICollection<User> GetUsers(XmlReader reader)
        {
            List<User> users = new List<User>();

            for (reader.ReadToDescendant(tagSubscribers);
                !(reader.Name == tagSubscribers && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                if(reader.IsStartElement(tagSubscriber))
                {
                    string subscriberType = reader.GetAttribute(attrSubscriberType);
                    string userName = reader.GetAttribute(attrSubscriberName);

                    switch (subscriberType)
                    {
                        case "user":

                            UserSearchCriteria criteria = new UserSearchCriteria();
                            criteria.UserName.EqualTo(userName);

                            IUserBroker broker = _context.GetBroker<IUserBroker>();
                            User user = CollectionUtils.FirstElement<User>(broker.Find(criteria));
                            if (user != null) users.Add(user);

                            break;
                        case "user-group":

                            AuthorityGroupSearchCriteria groupCriteria = new AuthorityGroupSearchCriteria();
                            groupCriteria.Name.EqualTo(userName);

                            IAuthorityGroupBroker groupBroker = _context.GetBroker<IAuthorityGroupBroker>();
                            AuthorityGroup group = CollectionUtils.FirstElement<AuthorityGroup>(groupBroker.Find(groupCriteria));

                            if (group != null)
                            {
                                foreach(User authorityGroupUser in group.Users)
                                {
                                    users.Add(authorityGroupUser);
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            return users;
        }

        //TODO: this doesn't actually work correctly because it allows creating "copies"
        //that are not valid (for example, a ReportingWorklist based on a PerformingGroup instead of a ReadingGroup)
        private IEnumerable<Worklist> GetWorklistCopies(XmlReader reader, string name)
        {
            List<Worklist> copies = new List<Worklist>();

            for (reader.ReadToDescendant(tagWorklistCopies);
                !(reader.Name == tagWorklistCopies && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                if (reader.IsStartElement(tagCopy))
                {
                    string copyType = reader.GetAttribute(attrCopyDiscriminator);
                    copies.Add(LoadOrCreateWorklist(name, copyType));
                }
            }

            return copies;
        }

        private Worklist LoadOrCreateWorklist(string name, string worklistClassName)
        {
            Worklist worklist;

            worklist = _context.GetBroker<IWorklistBroker>().FindWorklist(name, worklistClassName);

            if (worklist == null)
            {
                worklist = WorklistFactory.Instance.GetWorklist(worklistClassName);
                worklist.Name = name;

                _context.Lock(worklist, DirtyState.New);
            }

            return worklist;
        }
    }
}
