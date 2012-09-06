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
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="UserSessionManagmentComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class UserSessionManagmentComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }


    public class UserSessionSummaryTable : Table<UserSessionSummary>
    {
        #region Constructor

        public UserSessionSummaryTable()
        {
            Columns.Add(new TableColumn<UserSessionSummary, string>("Application", row => row.Application, 1.5f));
            Columns.Add(new TableColumn<UserSessionSummary, string>("Session ID", row => row.SessionId, 1f));
            Columns.Add(new TableColumn<UserSessionSummary, string>("Hostname", row => row.HostName, 1f));
            Columns.Add(new DateTimeTableColumn<UserSessionSummary>("Creation Time", row => row.CreationTime, 1f));
            Columns.Add(new DateTimeTableColumn<UserSessionSummary>("Expiry Time", row => row.ExpiryTime, 1f));
        }

        #endregion
    }
        

    [AssociateView(typeof(UserSessionManagmentComponentViewExtensionPoint))]
    public class UserSessionManagmentComponent : SummaryComponentBase<UserSessionSummary, UserSessionSummaryTable>
    {
        #region Constructors

        public  UserSessionManagmentComponent(UserSummary user)
        {
            Platform.CheckForNullReference(user, "user");
            
            User = user;
        }

        #endregion

        #region Presentation Model

        public UserSummary User { get; private set; }

		public void Close()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		#endregion

        #region Overridden Methods

		protected override bool SupportsAdd
		{
			get { return false; }
		}

		protected override bool SupportsDeactivation
		{
			get { return false; }
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		protected override bool SupportsEdit
		{
			get { return false; }
		}

		protected override bool SupportsPaging
		{
			get { return false; }
		}

		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			// modify the lable and tooltip on the delete button to be more appropriate for this context
			model.Delete.Label = SR.LabelTerminateSessions;
			model.Delete.Tooltip = SR.TooltipTerminateSessions;
		}

		protected override IList<UserSessionSummary> ListItems(int firstRow, int maxRows)
        {
            List<UserSessionSummary> sessions = null;
            Platform.GetService<IUserAdminService>(service => {
                sessions = service.ListUserSessions(new ListUserSessionsRequest(User.UserName)).Sessions.OrderBy(s => s.CreationTime).ToList();
            });               
            return sessions;
        }

        protected override bool AddItems(out IList<UserSessionSummary> addedItems)
        {
            throw new NotSupportedException("AddItems");
        }

        protected override bool EditItems(IList<UserSessionSummary> items, out IList<UserSessionSummary> editedItems)
        {
            throw new NotSupportedException("AddItems");
        }

		protected override string DeleteConfirmationMessage
		{
			get { return SR.MessageConfirmTerminateSelectedSessions; }
		}

        protected override bool DeleteItems(IList<UserSessionSummary> items, out IList<UserSessionSummary> deletedItems, out string failureMessage)
        {
        	List<string> terminatedSessionIds = null;
			Platform.GetService<IUserAdminService>(service => {
				var request = new TerminateUserSessionRequest(items.Select(s => s.SessionId).ToList());
				terminatedSessionIds = service.TerminateUserSession(request).TerminatedSessionIds;
			});
			deletedItems = terminatedSessionIds.Select(id => items.First(s => s.SessionId == id)).ToList();
        	failureMessage = null;
        	return true;
        }

        protected override bool IsSameItem(UserSessionSummary x, UserSessionSummary y)
        {
			return x.SessionId.Equals(y.SessionId);
        }

        #endregion
    }
}
