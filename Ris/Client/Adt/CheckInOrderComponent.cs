using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="CheckInOrderComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class CheckInOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// CheckInOrderComponent class
    /// </summary>
    [AssociateView(typeof(CheckInOrderComponentViewExtensionPoint))]
    public class CheckInOrderComponent : ApplicationComponent
    {
        private RegistrationWorklistItem _worklistItem;
        private CheckInOrderTable _checkInOrderTable;
        private List<EntityRef> _selectedOrders;

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckInOrderComponent(RegistrationWorklistItem item)
        {
            _worklistItem = item;
        }

        public override void Start()
        {
            _selectedOrders = new List<EntityRef>();
            _checkInOrderTable = new CheckInOrderTable();

            Platform.GetService<IRegistrationWorkflowService>(
                delegate(IRegistrationWorkflowService service)
                {
                    GetDataForCheckInTableResponse response = service.GetDataForCheckInTable(new GetDataForCheckInTableRequest(_worklistItem.PatientProfileRef));
                    _checkInOrderTable.Items.AddRange(
                        CollectionUtils.Map<CheckInTableItem, CheckInOrderTableEntry>(response.CheckInTableItems,
                                delegate(CheckInTableItem item)
                                {
                                    CheckInOrderTableEntry entry = new CheckInOrderTableEntry(item);
                                    entry.CheckedChanged += new EventHandler(OrderCheckedStateChangedEventHandler);
                                    return entry;
                                }));
                });

            CheckInOrderTableEntry selectedEntry = CollectionUtils.SelectFirst<CheckInOrderTableEntry>(_checkInOrderTable.Items,
                delegate(CheckInOrderTableEntry entry)
                {
                    return entry.CheckInTableItem.OrderRef == _worklistItem.OrderRef;
                });

            if (selectedEntry != null)
                selectedEntry.Checked = true;

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ITable OrderTable
        {
            get { return _checkInOrderTable; }
        }

        public List<EntityRef> SelectedOrders
        {
            get { return _selectedOrders; }
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
            // Get the list of Order EntityRef from the table
            foreach (CheckInOrderTableEntry entry in _checkInOrderTable.Items)
            {
                if (entry.Checked)
                    _selectedOrders.Add(entry.CheckInTableItem.OrderRef);
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

        private void OrderCheckedStateChangedEventHandler(object sender, EventArgs e)
        {
            foreach (CheckInOrderTableEntry entry in _checkInOrderTable.Items)
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
