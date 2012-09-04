#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Rules.GrantAccessAction
{
    
    /// <summary>
    /// <see cref="ServerDatabaseCommand"/> derived class for use with <see cref="ServerCommandProcessor"/> for inserting AutoRoute WorkQueue entries into the Persistent Store.
    /// </summary>
    public class InsertStudyDataAccessCommand : ServerDatabaseCommand
    {
        private readonly ServerActionContext _context;
        private readonly Guid _authorityGroupOid;
    	
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">A contentxt in which to apply the GrantAccess request.</param>
        /// <param name="authorityGroupOid">The OID of the Authority Group to grant access to the Study.</param>
        public InsertStudyDataAccessCommand(ServerActionContext context, Guid authorityGroupOid)
            : base("Update/Insert a StudyDataAccess Entry")
        {
            Platform.CheckForNullReference(context, "ServerActionContext");

            _context = context;
            _authorityGroupOid = authorityGroupOid;
        }

        /// <summary>
        /// Do the insertion of the AutoRoute.
        /// </summary>
        protected override void OnExecute(CommandProcessor theProcessor, IUpdateContext updateContext)
        {
            var criteria = new DataAccessGroupSelectCriteria();
            criteria.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID",_authorityGroupOid));

            var authorityGroup = updateContext.GetBroker<IDataAccessGroupEntityBroker>();

            DataAccessGroup group = authorityGroup.FindOne(criteria);
			if (group == null)
			{
				Platform.Log(LogLevel.Warn,
				             "AuthorityGroupOID '{0}' on partition {1} not in database for GrantAccess request!  Ignoring request.", _authorityGroupOid,
				             _context.ServerPartition.AeTitle);

                ServerPlatform.Alert(
                                AlertCategory.Application, AlertLevel.Warning,
                                SR.AlertComponentDataAccessRule, AlertTypeCodes.UnableToProcess, null, TimeSpan.FromMinutes(5),
                                SR.AlertDataAccessUnknownAuthorityGroup, _authorityGroupOid, _context.ServerPartition.AeTitle);
                return;
			}


            var entityBroker = updateContext.GetBroker<IStudyDataAccessEntityBroker>();

            var selectStudyDataAccess = new StudyDataAccessSelectCriteria();
            selectStudyDataAccess.DataAccessGroupKey.EqualTo(group.Key);
            selectStudyDataAccess.StudyStorageKey.EqualTo(_context.StudyLocationKey);

            if (entityBroker.Count(selectStudyDataAccess) == 0)
            {
                var insertColumns = new StudyDataAccessUpdateColumns
                                        {
                                            DataAccessGroupKey = group.Key,
                                            StudyStorageKey = _context.StudyLocationKey
                                        };

                entityBroker.Insert(insertColumns);
            }
        }
    }
}
