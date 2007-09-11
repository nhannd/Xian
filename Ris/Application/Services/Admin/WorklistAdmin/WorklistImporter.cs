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

        private const string tagRequestedProcedureTypeGroups = "requested-procedure-type-groups";
        private const string tagRequestedProcedureTypeGroup = "requested-procedure-type-group";
        private const string attrGroupName = "name";
        private const string attrGroupCategory = "category";

        private const string tagSubscribers = "subscribers";
        private const string tagSubscriber = "subscriber";
        private const string attrSubscriberName = "name";
        private const string attrSubscriberType = "type";

        private const string tagWorklistCopies = "worklist-copies";
        private const string tagCopy = "copy";
        private const string attrCopyDiscriminator = "discriminator";


        private IUpdateContext _context;

        #region DateImporterBase overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ImportXml(XmlReader reader, IUpdateContext context)
        {
            _context = context;

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

                reader.ReadToFollowing(tagRequestedProcedureTypeGroups);
                ICollection requestedProcedureTypeGroups = GetRequestedProcedureTypeGroups(reader.ReadSubtree());

                reader.ReadToFollowing(tagSubscribers);
                ICollection users = GetUsers(reader.ReadSubtree());

                worklist.Description = description;
                worklist.RequestedProcedureTypeGroups.AddAll(requestedProcedureTypeGroups);
                worklist.Users.AddAll(users);

                reader.ReadToFollowing(tagWorklistCopies);
                foreach (Worklist copy in GetWorklistCopies(reader.ReadSubtree(), name))
                {
                    copy.Description = description;
                    copy.RequestedProcedureTypeGroups.AddAll(requestedProcedureTypeGroups);
                    copy.Users.AddAll(users);
                }
            }
        }

        private ICollection GetRequestedProcedureTypeGroups(XmlReader reader)
        {
            List<RequestedProcedureTypeGroup> groups = new List<RequestedProcedureTypeGroup>();

            for (reader.ReadToDescendant(tagRequestedProcedureTypeGroups);
                !(reader.Name == tagRequestedProcedureTypeGroups && reader.NodeType == XmlNodeType.EndElement);
                reader.Read())
            {
                if (reader.IsStartElement(tagRequestedProcedureTypeGroup))
                {
                    RequestedProcedureTypeGroupSearchCriteria criteria = new RequestedProcedureTypeGroupSearchCriteria();
                    criteria.Name.EqualTo(reader.GetAttribute(attrGroupName));
                    try
                    {
                        RequestedProcedureTypeGroupCategory category =
                            (RequestedProcedureTypeGroupCategory)
                            Enum.Parse(typeof (RequestedProcedureTypeGroupCategory),
                                       reader.GetAttribute(attrGroupCategory));

                        criteria.Category.EqualTo(category);
                    }
                    catch(Exception)
                    {
                        // Ignore category
                    }

                    IRequestedProcedureTypeGroupBroker broker = _context.GetBroker<IRequestedProcedureTypeGroupBroker>();
                    RequestedProcedureTypeGroup group = CollectionUtils.FirstElement<RequestedProcedureTypeGroup>(broker.Find(criteria));
                    if (group != null) groups.Add(group);
                }
            }

            return groups;
        }

        private ICollection GetUsers(XmlReader reader)
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
                    string copyName = name;
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
                worklist = CreateWorklist(worklistClassName);
                worklist.Name = name;

                _context.Lock(worklist, DirtyState.New);
            }

            return worklist;
        }

        private Worklist CreateWorklist(string worklistClassName)
        {
            Worklist worklist = null;

            switch (worklistClassName)
            {
                case "RegistrationScheduledWorklist":
                    worklist = new RegistrationScheduledWorklist();
                    break;
                case "RegistrationCheckedInWorklist":
                    worklist = new RegistrationCheckedInWorklist();
                    break;
                case "RegistrationInProgressWorklist":
                    worklist = new RegistrationInProgressWorklist();
                    break;
                case "RegistrationCancelledWorklist":
                    worklist = new RegistrationCancelledWorklist();
                    break;
                case "RegistrationCompletedWorklist":
                    worklist = new RegistrationCompletedWorklist();
                    break;
                case "TechnologistScheduledWorklist":
                    worklist = new TechnologistScheduledWorklist();
                    break;
                case "TechnologistCheckedInWorklist":
                    worklist = new TechnologistCheckedInWorklist();
                    break;
                case "TechnologistInProgressWorklist":
                    worklist = new TechnologistInProgressWorklist();
                    break;
                case "TechnologistCancelledWorklist":
                    worklist = new TechnologistCancelledWorklist();
                    break;
                case "TechnologistCompletedWorklist":
                    worklist = new TechnologistCompletedWorklist();
                    break;
                case "ReportingToBeReportedWorklist":
                    worklist = new ReportingToBeReportedWorklist();
                    break;
                default:
                    break;
            }

            return worklist;
        }

    }
}
