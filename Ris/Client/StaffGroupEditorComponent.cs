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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="StaffGroupEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class StaffGroupEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// StaffGroupEditorComponent class
    /// </summary>
    [AssociateView(typeof(StaffGroupEditorComponentViewExtensionPoint))]
    public class StaffGroupEditorComponent : ApplicationComponent
    {
        class StaffTable : Table<StaffSummary>
        {
            public StaffTable()
            {
                this.Columns.Add(new TableColumn<StaffSummary, string>("Name",
                    delegate(StaffSummary item) { return PersonNameFormat.Format(item.Name); }, 1.0f));
                this.Columns.Add(new TableColumn<StaffSummary, string>("Role",
                    delegate(StaffSummary item) { return item.StaffType.Value; }, 0.5f));
            }
        }

        private EntityRef _staffGroupRef;
        private StaffGroupDetail _staffGroupDetail;

        private StaffTable _availableStaff;
        private StaffTable _selectedStaff;

        // return value
        private StaffGroupSummary _staffGroupSummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffGroupEditorComponent()
        {
        }

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        public StaffGroupEditorComponent(EntityRef staffGroupRef)
        {
            _staffGroupRef = staffGroupRef;
        }

        public StaffGroupSummary StaffGroupSummary
        {
            get { return _staffGroupSummary; }
        }

        public override void Start()
        {
            _availableStaff = new StaffTable();
            _selectedStaff = new StaffTable();

            Platform.GetService<IStaffGroupAdminService>(
                delegate(IStaffGroupAdminService service)
                {
                    LoadStaffGroupEditorFormDataResponse formDataResponse = service.LoadStaffGroupEditorFormData(
                        new LoadStaffGroupEditorFormDataRequest());

                    if (_staffGroupRef == null)
                    {
                        _staffGroupDetail = new StaffGroupDetail();
                    }
                    else
                    {
                        LoadStaffGroupForEditResponse response = service.LoadStaffGroupForEdit(new LoadStaffGroupForEditRequest(_staffGroupRef));
                        _staffGroupRef = response.StaffGroup.StaffGroupRef;
                        _staffGroupDetail = response.StaffGroup;
                    }

                    _selectedStaff.Items.AddRange(_staffGroupDetail.Members);
                    _availableStaff.Items.AddRange(CollectionUtils.Reject(formDataResponse.AllStaff,
                        delegate(StaffSummary x)
                        {
                            return CollectionUtils.Contains(_staffGroupDetail.Members,
                                delegate(StaffSummary y) { return x.StaffRef.Equals(y.StaffRef, true); });
                        }));
                });

            base.Start();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string GroupName
        {
            get { return _staffGroupDetail.Name; }
            set
            {
                if(_staffGroupDetail.Name != value)
                {
                    _staffGroupDetail.Name = value;
                    this.Modified = true;
                    NotifyPropertyChanged("GroupName");
                }
            }
        }

        public string GroupDescription
        {
            get { return _staffGroupDetail.Description; }
            set
            {
                if (_staffGroupDetail.Description != value)
                {
                    _staffGroupDetail.Description = value;
                    this.Modified = true;
                    NotifyPropertyChanged("GroupDescription");
                }
            }
        }

		public bool IsElective
		{
			get { return _staffGroupDetail.IsElective; }
			set
			{
				if (_staffGroupDetail.IsElective != value)
				{
					_staffGroupDetail.IsElective = value;
					this.Modified = true;
					NotifyPropertyChanged("IsElective");
				}
			}
		}

        public ITable AvailableStaffTable
        {
            get { return _availableStaff; }
        }

        public ITable SelectedStaffTable
        {
            get { return _selectedStaff; }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                _staffGroupDetail.Members = new List<StaffSummary>(_selectedStaff.Items);

                Platform.GetService<IStaffGroupAdminService>(
                    delegate(IStaffGroupAdminService service)
                    {
                        if (_staffGroupRef == null)
                        {
                            AddStaffGroupResponse response = service.AddStaffGroup(
                                new AddStaffGroupRequest(_staffGroupDetail));
                            _staffGroupRef = response.StaffGroup.StaffGroupRef;
                            _staffGroupSummary = response.StaffGroup;
                        }
                        else
                        {
                            UpdateStaffGroupResponse response = service.UpdateStaffGroup(
                                new UpdateStaffGroupRequest(_staffGroupDetail));
                            _staffGroupRef = response.StaffGroup.StaffGroupRef;
                            _staffGroupSummary = response.StaffGroup;
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, "Unable to save Staff Group", this.Host.DesktopWindow,
                    delegate()
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion
    }
}
