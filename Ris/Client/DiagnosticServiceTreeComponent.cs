#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="DiagnosticServiceTreeComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DiagnosticServiceTreeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DiagnosticServiceTreeComponent class
    /// </summary>
    [AssociateView(typeof(DiagnosticServiceTreeComponentViewExtensionPoint))]
    public class DiagnosticServiceTreeComponent : ApplicationComponent
    {
        private Tree<DiagnosticServiceTreeItem> _diagnosticServiceTree;
        private DiagnosticServiceTreeItem _selectedDiagnosticServiceTreeItem;
        private DiagnosticServiceDetail _selectedDiagnosticServiceDetail;

        private Table<RequestedProcedureTypeDetail> _diagnosticServiceBreakdown;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiagnosticServiceTreeComponent()
        {
            _diagnosticServiceBreakdown = new Table<RequestedProcedureTypeDetail>();
            _diagnosticServiceBreakdown.Columns.Add(
                new TableColumn<RequestedProcedureTypeDetail, string>("Procedure Name",
                           delegate(RequestedProcedureTypeDetail rp) { return rp.Name; }));
        }

        public DiagnosticServiceDetail SelectedDiagnosticServiceDetail
        {
            get { return _selectedDiagnosticServiceDetail; }
        }

        public override void Start()
        {
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    _diagnosticServiceTree = ExpandDiagnosticServiceTree(null);
                });


            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITree DiagnosticServiceTree
        {
            get { return _diagnosticServiceTree; }
        }

        public ISelection SelectedDiagnosticServiceTreeItem
        {
            get { return new Selection(_selectedDiagnosticServiceTreeItem); }
            set
            {
                _selectedDiagnosticServiceTreeItem = value.Item as DiagnosticServiceTreeItem;
                UpdateDiagnosticServiceBreakdown();
            }
        }

        public ITable DiagnosticServiceBreakdown
        {
            get { return _diagnosticServiceBreakdown; }
        }

        public bool AcceptEnabled
        {
            get { return _selectedDiagnosticServiceDetail != null; }
        }

        public void Accept()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion


        private Tree<DiagnosticServiceTreeItem> ExpandDiagnosticServiceTree(DiagnosticServiceTreeItem item)
        {
            Tree<DiagnosticServiceTreeItem> subtree = null;

            try
            {
                Platform.GetService<IOrderEntryService>(
                    delegate(IOrderEntryService service)
                    {
                        EntityRef nodeRef = item == null ? null : item.NodeRef;
                        GetDiagnosticServiceSubTreeResponse response = service.GetDiagnosticServiceSubTree(new GetDiagnosticServiceSubTreeRequest(nodeRef));

                        TreeItemBinding<DiagnosticServiceTreeItem> binding = new TreeItemBinding<DiagnosticServiceTreeItem>(
                                delegate(DiagnosticServiceTreeItem ds) { return ds.Description; },
                                ExpandDiagnosticServiceTree);
                        binding.CanHaveSubTreeHandler = delegate(DiagnosticServiceTreeItem ds) { return ds.DiagnosticService == null; };
                        subtree = new Tree<DiagnosticServiceTreeItem>(binding, response.DiagnosticServiceSubTree);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotExpandDiagnositicServiceTree, this.Host.DesktopWindow,
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }

            return subtree;
        }

        private void UpdateDiagnosticServiceBreakdown()
        {
            _selectedDiagnosticServiceDetail = null;
            _diagnosticServiceBreakdown.Items.Clear();


            if (_selectedDiagnosticServiceTreeItem != null && _selectedDiagnosticServiceTreeItem.DiagnosticService != null)
            {
                try
                {
                    Platform.GetService<IOrderEntryService>(
                        delegate(IOrderEntryService service)
                        {
                            LoadDiagnosticServiceBreakdownResponse response = service.LoadDiagnosticServiceBreakdown(new LoadDiagnosticServiceBreakdownRequest(_selectedDiagnosticServiceTreeItem.DiagnosticService.DiagnosticServiceRef));
                            _selectedDiagnosticServiceDetail = response.DiagnosticServiceDetail;

                            _diagnosticServiceBreakdown.Items.AddRange(_selectedDiagnosticServiceDetail.RequestedProcedureTypes);
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionCannotUpdateDiagnosticServiceBreakdown,
                                            this.Host.DesktopWindow);
                }
            }

            NotifyPropertyChanged("AcceptEnabled");
        }

    }
}
