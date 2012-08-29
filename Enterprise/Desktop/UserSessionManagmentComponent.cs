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
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

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
        #region Public Properties

        public UserSessionManagmentComponent Component;

        #endregion

        #region Constructor

        public UserSessionSummaryTable()
        {
            Columns.Add(new TableColumn<UserSessionSummary, string>("Application", row => row.Application, 1.5f));
            Columns.Add(new TableColumn<UserSessionSummary, string>("Session ID", row => row.SessionID, 1f));
            Columns.Add(new TableColumn<UserSessionSummary, string>("Hostname", row => row.HostName, 1f));
            Columns.Add(new DateTimeTableColumn<UserSessionSummary>("Creation Time", row => row.CreationTime, 1f));
            Columns.Add(new DateTimeTableColumn<UserSessionSummary>("Expiry Time", row => row.ExpiryTime, 1f));
            Columns.Add(new TableColumn<UserSessionSummary, string>(" ", session => "Terminate", 0.7f) { ClickLinkDelegate = OnSessionItemClicked });
        }

        #endregion

        #region Private Methods

        private void OnSessionItemClicked(UserSessionSummary session)
        {
            if (Component != null)
            {
                Component.TerminateSession(session.SessionID);
            }
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

        #region Public Properties

        public UserSummary User { get; private set; }

        public override ITable SummaryTable
        {
            get
            {
                if (base.SummaryTable != null)
                {
                    (base.SummaryTable as UserSessionSummaryTable).Component = this;
                }

                return base.SummaryTable;
            }
        }

        protected override bool SupportsAdd { get { return false; } }

        protected override bool SupportsDeactivation
        {
            get
            {
                return false;
            }
        }

        protected override bool SupportsDelete
        {
            get
            {
                return false;
            }
        }

        protected override bool SupportsEdit
        {
            get
            {
                return false;
            }
        }

        public override ActionModelNode SummaryTableActionModel
        {
            get
            {
                // no RCCM 
                return null;
            }
        }

        #endregion

        #region Overridden Methods

        protected override IList<UserSessionSummary> ListItems(int firstRow, int maxRows)
        {
            List<UserSessionSummary> list = null ;
            try
            {
                
                Platform.GetService<IUserSessionAdminService>(service =>
                {
                    var response = service.ListUserSessions(new ListUserSessionsRequest() { UserName = User.UserName });
                    list = response.Sessions;
                });               
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, this.Host.DesktopWindow);
            }
            
            if (list==null)
                return new List<UserSessionSummary>();

            list.Sort(SortByTime);
            return list;
        }

        protected override bool AddItems(out IList<UserSessionSummary> addedItems)
        {
            throw new NotSupportedException("AddItems");
        }

        protected override bool EditItems(IList<UserSessionSummary> items, out IList<UserSessionSummary> editedItems)
        {
            throw new NotSupportedException("AddItems");
        }

        protected override bool DeleteItems(IList<UserSessionSummary> items, out IList<UserSessionSummary> deletedItems, out string failureMessage)
        {
            throw new NotImplementedException();
        }

        protected override bool IsSameItem(UserSessionSummary x, UserSessionSummary y)
        {
            return x.SessionID.Equals(y.SessionID);
        }

        #endregion

        #region Private Methods

        private int SortByTime(UserSessionSummary x, UserSessionSummary y)
        {
            return x.ExpiryTime.CompareTo(y.ExpiryTime);
        }

        private void TerminateSessionInternal(string sessionID)
        {
            try
            {

                Platform.GetService<IUserSessionAdminService>(service =>
                {
                    var response = service.TerminateUserSession(new TerminateUserSessionRequest()
                    {
                        SessionIDs = new List<string>(new[]{sessionID})
                    });

                });

            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, this.Host.DesktopWindow);
            }
        }

        #endregion

        #region Public Methods

        public void TerminateSession(string sessionID)
        {
            TerminateSessionInternal(sessionID);
            base.Search();
        }

        public void TerminateSelectedSessions()
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                CollectionUtils.ForEach(SelectedItems, session => TerminateSessionInternal(session.SessionID));
                base.Search();
            }
        }

        #endregion
    }
}
