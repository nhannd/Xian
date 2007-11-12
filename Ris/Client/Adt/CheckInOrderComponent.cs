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
                    this.Exit(ApplicationComponentExitCode.Accepted);
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
            this.ExitCode = ApplicationComponentExitCode.None;
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
