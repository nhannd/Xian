#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using System.Security.Permissions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.Threading;

namespace ClearCanvas.Enterprise.Authentication.Admin.UserAdmin
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
	[ServiceImplementsContract(typeof(IUserSessionAdminService))]
    internal class UserSessionAdminService : CoreServiceLayer, IUserSessionAdminService
    {
        #region IUserSessionAdminService Members

        [ReadOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
        public ListUserSessionsResponse ListUserSessions(ListUserSessionsRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.UserName, "UserName");

            var user = FindUserByName(request.UserName);

            UserAssembler assembler = new UserAssembler();
            return new ListUserSessionsResponse()
            {
                UserName = user.UserName,
                Sessions = assembler.GetUserSessionSummaries(user)
            };
        }


        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Admin.Security.User)]
        public TerminateUserSessionResponse TerminateUserSession(TerminateUserSessionRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckForNullReference(request.SessionIDs, "SessionIDs");

            if (IsCurrentSessionPresent(request.SessionIDs))
            {
                throw new RequestValidationException(SR.MessageCannotDeleteOwnUserCurrentSession);
            }

            foreach (var sessionID in request.SessionIDs)
            {
                var session = FindUserSessionByID(sessionID);
                if (session != null && !session.IsTerminated)
                {
                    try
                    {
                        session.Terminate();
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Error occurred when trying to terminate session (ID={0})", sessionID);
                        throw new RequestValidationException(ex.Message);
                    }
                }
            }

            return new TerminateUserSessionResponse();
        }

        #endregion

        #region Private Methods

        private User FindUserByName(string name)
        {
            try
            {
                UserSearchCriteria where = new UserSearchCriteria();
                where.UserName.EqualTo(name);

                return PersistenceContext.GetBroker<IUserBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                throw new RequestValidationException(string.Format("{0} is not a valid user name.", name));
            }
        }

        private bool IsCurrentSessionPresent(List<string> sessionIDs)
        {

            var userSessionID = string.Empty;

            if (Thread.CurrentPrincipal is DefaultPrincipal)
            {
                userSessionID = (Thread.CurrentPrincipal as DefaultPrincipal).SessionToken.Id;

                return sessionIDs.Contains(userSessionID);
            }

            return false;
        }

        private UserSession FindUserSessionByID(string sessionID)
        {
            try
            {
                UserSessionSearchCriteria where = new UserSessionSearchCriteria();
                where.SessionId.EqualTo(sessionID);

                return PersistenceContext.GetBroker<IUserSessionBroker>().FindOne(where);
            }
            catch (EntityNotFoundException)
            {
                throw new RequestValidationException(string.Format("{0} is not a valid user session ID.", sessionID));
            }
        }

        #endregion
    }
}