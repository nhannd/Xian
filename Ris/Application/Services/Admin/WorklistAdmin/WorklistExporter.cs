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
using System.Collections.Generic;
using System.Xml;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Authentication;


namespace ClearCanvas.Ris.Application.Services.Admin.WorklistAdmin
{
    [ExtensionOf(typeof(DataExporterExtensionPoint))]
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class WorklistExporter : DataExporterBase
    {
        private IReadContext _context;

        private const string tagWorklists = "worklists";

        private const string tagWorklist = "worklist";
        private const string attrName = "name";
        private const string attrDiscriminator = "discriminator";
        private const string attrDescription = "description";

        private const string tagRequestedProcedureTypeGroups = "requested-procedure-type-groups";
        private const string tagRequestedProcedureTypeGroup = "requested-procedure-type-group";
        private const string attrRptName = "name";
        private const string attrRptCategory = "category";

        private const string tagSubscribers = "subscribers";
        private const string tagSubscriber = "subscriber";
        private const string attrSubscriberName = "name";
        private const string attrSubscriberType = "type";
        private const string subscriberType = "user";

        #region DateExporter overrides

        public override bool SupportsXml
        {
            get { return true; }
        }

        public override void ExportXml(XmlWriter writer, IReadContext context)
        {
            _context = context;

            writer.WriteStartDocument();
            writer.WriteStartElement(tagWorklists);

            IList<Worklist> worklists = context.GetBroker<IWorklistBroker>().FindAll();
            CollectionUtils.ForEach<Worklist>(worklists,
                delegate(Worklist worklist) { WriteWorklistXml(worklist, writer); });

            writer.WriteEndElement();
        }

        #endregion

        #region Private methods

        private void WriteWorklistXml(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagWorklist);
            writer.WriteAttributeString(attrName, worklist.Name);
            writer.WriteAttributeString(attrDiscriminator, GetDiscriminator(worklist));
            writer.WriteAttributeString(attrDescription, worklist.Description);

            WriteRequestedProcedureTypeGroupsXml(worklist, writer);
            WriteSubscribersXml(worklist, writer);

            writer.WriteEndElement();
        }

        private void WriteSubscribersXml(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagSubscribers);
            CollectionUtils.ForEach<User>(worklist.Users,
                delegate(User user) { WriteSubscriberXml(user, writer); });
            writer.WriteEndElement();
        }

        private void WriteSubscriberXml(User user, XmlWriter writer)
        {
            writer.WriteStartElement(tagSubscriber);
            writer.WriteAttributeString(attrSubscriberName, user.UserName);
            writer.WriteAttributeString(attrSubscriberType, subscriberType);
            writer.WriteEndElement();
        }

        private void WriteRequestedProcedureTypeGroupsXml(Worklist worklist, XmlWriter writer)
        {
            writer.WriteStartElement(tagRequestedProcedureTypeGroups);
            CollectionUtils.ForEach<RequestedProcedureTypeGroup>(worklist.RequestedProcedureTypeGroups,
                delegate(RequestedProcedureTypeGroup group) { WriteRequestedProcedureTypeGroupXml(group, writer); });
            writer.WriteEndElement();

        }

        private void WriteRequestedProcedureTypeGroupXml(RequestedProcedureTypeGroup group, XmlWriter writer)
        {
            writer.WriteStartElement(tagRequestedProcedureTypeGroup);
            writer.WriteAttributeString(attrRptName, group.Name);
            writer.WriteAttributeString(attrRptCategory, group.Category.ToString());
            writer.WriteEndElement();
        }

        private string GetDiscriminator(Worklist worklist)
        {
            return worklist.GetType().Name;
        }

        #endregion
    }
}
