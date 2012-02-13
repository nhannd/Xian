#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageServer.Common
{
    public static class ServerAuditHelper
    {

        public static void AddAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
                                                      EventIdentificationContentsEventActionCode.U,
                                                      EventTypeCode.ObjectSecurityAttributesChanged);

            // TODO: 8/19/2011, Develop a way to get the DisplayName for the user here for the audit log message
            helper.AddUser(new AuditPersonActiveParticipant(
                               Thread.CurrentPrincipal.Identity.Name,
                               null,
                               null));

            var participant = new AuditStudyParticipantObject(studyInstanceUid, accessionNumber);

            string updateDescription = StringUtilities.Combine(
                assignedGroups, ";",
                item => String.Format("Assigned Group Access=\"{0}\"", item)
                );

            participant.ParticipantObjectDetailString = updateDescription;
            helper.AddStudyParticipantObject(participant);


            ServerPlatform.LogAuditMessage(helper);
        }

        public static void RemoveAuthorityGroupAccess(string studyInstanceUid, string accessionNumber, IList<string> assignedGroups)
        {
            Platform.CheckForNullReference(studyInstanceUid, "studyInstanceUid");
            Platform.CheckForNullReference(assignedGroups, "assignedGroups");

            var helper =
                new DicomInstancesAccessedAuditHelper(ServerPlatform.AuditSource,
                                                      EventIdentificationContentsEventOutcomeIndicator.Success,
                                                      EventIdentificationContentsEventActionCode.U,
                                                      EventTypeCode.ObjectSecurityAttributesChanged);

            // TODO: 8/19/2011, Develop a way to get the DisplayName for the user here for the audit log message
            helper.AddUser(new AuditPersonActiveParticipant(
                               Thread.CurrentPrincipal.Identity.Name,
                               null,
                               null));


            var participant = new AuditStudyParticipantObject(studyInstanceUid, accessionNumber);

            string updateDescription = StringUtilities.Combine(
                assignedGroups, ";",
                item => String.Format("Removed Group Access=\"{0}\"", item)
                );

            participant.ParticipantObjectDetailString = updateDescription;
            helper.AddStudyParticipantObject(participant);

            ServerPlatform.LogAuditMessage(helper);
        }
    }
}
