#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
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
        public EnumValueInfo Category
        {
            get { return _editedItemDetail.Category; }
            set
            {
                _editedItemDetail.Category = value;
                this.Modified = true;
            }
        }

        public IList CategoryChoices
        {
            get { return _procedureTypeGroupCategoryChoices; }
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
