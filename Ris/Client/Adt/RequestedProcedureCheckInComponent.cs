using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="RequestedProcedureCheckInComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RequestedProcedureCheckInComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RequestedProcedureCheckInComponent class
    /// </summary>
    [AssociateView(typeof(RequestedProcedureCheckInComponentViewExtensionPoint))]
    public class RequestedProcedureCheckInComponent : ApplicationComponent
    {
        private WorklistItem _worklistItem;
        private IWorklistService _worklistService;
        private RequestedProcedureCheckInTable _requestedProcedureCheckInTable;

        /// <summary>
        /// Constructor
        /// </summary>
        public RequestedProcedureCheckInComponent(WorklistItem item)
        {
            _worklistItem = item;
        }

        public override void Start()
        {
            _requestedProcedureCheckInTable = new RequestedProcedureCheckInTable();

            IDictionary<EntityRef<RequestedProcedure>, RequestedProcedureCheckInTableEntry> dictionary = new Dictionary<EntityRef<RequestedProcedure>, RequestedProcedureCheckInTableEntry>();

            _worklistService = ApplicationContext.GetService<IWorklistService>();
            IList<WorklistQueryResult> listQueryResult = (IList<WorklistQueryResult>)_worklistService.GetQueryResultForWorklistItem("ClearCanvas.Healthcare.Workflow.Registration.Worklists+Scheduled", _worklistItem);

            foreach (WorklistQueryResult queryResult in listQueryResult)
            {
                if (dictionary.ContainsKey(queryResult.RequestedProcedure) == false)
                {
                    RequestedProcedure rp = _worklistService.LoadRequestedProcedure(queryResult.RequestedProcedure, true);
                    RequestedProcedureCheckInTableEntry entry = new RequestedProcedureCheckInTableEntry(rp);
                    entry.CheckedChanged += new EventHandler(RequestedProcedureCheckedStateChangedEventHandler);
                    _requestedProcedureCheckInTable.Items.Add(entry);
                    
                    dictionary[queryResult.RequestedProcedure] = entry;
                }
            }

            // Special case for 0 or 1 Requested Procedure.  No need to show the dialog box
            if (_requestedProcedureCheckInTable.Items.Count == 0)
            {
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
            }
            else if (_requestedProcedureCheckInTable.Items.Count == 1)
            {
                _requestedProcedureCheckInTable.Items[0].Checked = true;
                SaveChanges();
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable RequestedProcedureTable
        {
            get { return _requestedProcedureCheckInTable; }
        }

        #endregion

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                try
                {
                    SaveChanges();
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private void SaveChanges()
        {
            // TODO: Need to get the real current staff that is using the system
            IStaffAdminService staffAdminService = ApplicationContext.GetService<IStaffAdminService>();
            Staff staff = new Staff(new PersonName("Clerk", "Registration"));
            IList<Staff> listStaff = staffAdminService.FindStaffs(staff.Name.FamilyName, staff.Name.GivenName);
            if (listStaff.Count == 0)
                staffAdminService.AddStaff(staff);
            else
                staff = CollectionUtils.FirstElement(listStaff) as Staff;

            foreach (RequestedProcedureCheckInTableEntry entry in _requestedProcedureCheckInTable.Items)
            {
                if (entry.Checked)
                {
                    CheckInProcedureStep cps = new CheckInProcedureStep(entry.RequestedProcedure);
                    cps.Start(staff);
                    cps.Complete(staff);
                    entry.RequestedProcedure.CheckInProcedureSteps.Add(cps);
                    _worklistService.UpdateRequestedProcedure(entry.RequestedProcedure);
                }
            }      
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        private void RequestedProcedureCheckedStateChangedEventHandler(object sender, EventArgs e)
        {
            foreach (RequestedProcedureCheckInTableEntry entry in _requestedProcedureCheckInTable.Items)
            {
                if (entry.Checked)
                {
                    this.Modified = true;
                    return;
                }
            }

            this.Modified = false;
        }

    }
}
