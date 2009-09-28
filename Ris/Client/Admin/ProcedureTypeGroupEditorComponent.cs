#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProcedureTypeGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProcedureTypeGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProcedureTypeGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProcedureTypeGroupEditorComponentViewExtensionPoint))]
    public class ProcedureTypeGroupEditorComponent : ApplicationComponent
    {
        private readonly bool _isNew;

        private EntityRef _editedItemEntityRef;
        private ProcedureTypeGroupDetail _editedItemDetail;
        private ProcedureTypeGroupSummary _editedItemSummary;

        private List<EnumValueInfo> _procedureTypeGroupCategoryChoices;
        private ProcedureTypeSummaryTable _availableProcedureTypes;
        private ProcedureTypeSummaryTable _selectedProcedureTypes;

        public ProcedureTypeGroupEditorComponent()
        {
            _isNew = true;
        }

        public ProcedureTypeGroupEditorComponent(EntityRef entityRef)
        {
            _editedItemEntityRef = entityRef;
            _isNew = false;
        }

        public override void Start()
        {
            _availableProcedureTypes = new ProcedureTypeSummaryTable();
            _selectedProcedureTypes = new ProcedureTypeSummaryTable();

            Platform.GetService<IProcedureTypeGroupAdminService>(
                delegate(IProcedureTypeGroupAdminService service)
                    {
                        GetProcedureTypeGroupEditFormDataResponse formDataResponse =
                            service.GetProcedureTypeGroupEditFormData(new GetProcedureTypeGroupEditFormDataRequest());
                        _procedureTypeGroupCategoryChoices = formDataResponse.Categories;
                        _availableProcedureTypes.Items.AddRange(formDataResponse.ProcedureTypes);

                        if (_isNew)
                        {
                            _editedItemDetail = new ProcedureTypeGroupDetail();
                            _editedItemDetail.Category = _procedureTypeGroupCategoryChoices[0];
                        }
                        else
                        {
                            LoadProcedureTypeGroupForEditResponse response =
                                service.LoadProcedureTypeGroupForEdit(
                                    new LoadProcedureTypeGroupForEditRequest(_editedItemEntityRef));

                            _editedItemEntityRef = response.EntityRef;
                            _editedItemDetail = response.Detail;
                            _selectedProcedureTypes.Items.AddRange(_editedItemDetail.ProcedureTypes);
                        }

                        foreach(ProcedureTypeSummary selectedSummary in _selectedProcedureTypes.Items)
                        {
                            _availableProcedureTypes.Items.Remove(selectedSummary);
                        }
                    });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ProcedureTypeGroupSummary ProcedureTypeGroupSummary
        {
            get { return _editedItemSummary; }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _editedItemDetail.Name; }
            set
            {
                _editedItemDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _editedItemDetail.Description; }
            set
            {
                _editedItemDetail.Description = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Category
        {
            get { return _editedItemDetail.Category == null ? string.Empty : _editedItemDetail.Category.Value; }
            set
            {
                _editedItemDetail.Category = EnumValueUtils.MapDisplayValue(_procedureTypeGroupCategoryChoices, value);
                this.Modified = true;
            }
        }

        public IList<string> CategoryChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_procedureTypeGroupCategoryChoices); }
        }

        public bool CategoryEnabled
        {
            get { return _isNew; }
        }

        public ITable AvailableProcedureTypes
        {
            get { return _availableProcedureTypes; }
        }

        public ITable SelectedProcedureTypes
        {
            get { return _selectedProcedureTypes; }
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
                    _editedItemDetail.ProcedureTypes.Clear();
                    _editedItemDetail.ProcedureTypes.AddRange(_selectedProcedureTypes.Items);

                    Platform.GetService<IProcedureTypeGroupAdminService>(
                        delegate(IProcedureTypeGroupAdminService service)
                        {
                            if (_isNew)
                            {
                                AddProcedureTypeGroupResponse response = 
                                    service.AddProcedureTypeGroup(new AddProcedureTypeGroupRequest(_editedItemDetail));
                                _editedItemEntityRef = response.AddedProcedureTypeGroupSummary.ProcedureTypeGroupRef;
                                _editedItemSummary = response.AddedProcedureTypeGroupSummary;
                            }
                            else
                            {
                                UpdateProcedureTypeGroupResponse response = 
                                    service.UpdateProcedureTypeGroup(new UpdateProcedureTypeGroupRequest(_editedItemEntityRef, _editedItemDetail));
								_editedItemEntityRef = response.UpdatedProcedureTypeGroupSummary.ProcedureTypeGroupRef;
                                _editedItemSummary = response.UpdatedProcedureTypeGroupSummary;
                            }
                        });

                    this.Exit(ApplicationComponentExitCode.Accepted);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveProcedureTypeGroup, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public void ItemsAddedOrRemoved()
        {
            this.Modified = true;
        }
    }
}
