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

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistDetailEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistDetailEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistDetailEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistDetailEditorComponentViewExtensionPoint))]
    public class WorklistDetailEditorComponent : WorklistDetailEditorComponentBase
    {
        private readonly WorklistAdminDetail _worklistDetail;
        private readonly WorklistEditorMode _editorMode;
        private readonly bool _dialogMode;
    	private readonly bool _adminMode;
		private readonly List<StaffGroupSummary> _groupChoices;

    	private bool _isPersonal;

        /// <summary>
        /// Constructor
        /// </summary>
		public WorklistDetailEditorComponent(WorklistAdminDetail detail, List<WorklistClassSummary> worklistClasses, List<StaffGroupSummary> ownerGroupChoices, WorklistEditorMode editorMode, bool adminMode, bool dialogMode)
			:base(worklistClasses, GetDefaultWorklistClass(worklistClasses, detail))
        {
            _worklistDetail = detail;
            _dialogMode = dialogMode;
            _editorMode = editorMode;
			_adminMode = adminMode;
        	_groupChoices = ownerGroupChoices;

			if(_editorMode == WorklistEditorMode.Add)
			{
				// default to "personal" if user has authority
				_isPersonal = HasPersonalAdminAuthority;
			}
			else
			{
				// default to "personal" if not a user worklist (this could happen when duplicating from an admin worklist)
				_isPersonal = !_worklistDetail.IsUserWorklist || _worklistDetail.IsStaffOwned;
			}

			// update the class to the default (if this is a new worklist)
        	_worklistDetail.WorklistClass = GetDefaultWorklistClass(worklistClasses, detail);

            this.Validation.Add(
                new ValidationRule("SelectedGroup",
                    delegate
                    {
                        bool success = _adminMode || this.IsPersonal || (this.IsGroup && this.SelectedGroup != null);
                        return new ValidationResult(success, "Value Required");
                    }));
		}

    	#region Presentation Model

    	public bool IsOwnerPanelVisible
    	{
			get { return !_adminMode; }
    	}

    	public bool IsPersonalGroupSelectionEnabled
    	{
			get { return _editorMode != WorklistEditorMode.Edit && HasGroupAdminAuthority && HasPersonalAdminAuthority; }
    	}

		public bool IsPersonal
		{
			get { return _isPersonal; }
			set
			{
				if(value != _isPersonal)
				{
					_isPersonal = value;
					if (_isPersonal)
						this.SelectedGroup = null;

					this.Modified = true;
					NotifyPropertyChanged("IsPersonal");
					NotifyPropertyChanged("IsGroup");
				}
			}
		}

		public bool IsGroup
		{
			get { return !this.IsPersonal; }
			set { this.IsPersonal = !value; }
		}

    	public bool IsGroupChoicesEnabled
    	{
            get { return _editorMode != WorklistEditorMode.Edit && HasGroupAdminAuthority && IsGroup; }
    	}

    	public IList GroupChoices
    	{
			get { return _groupChoices; }
    	}

		public string FormatGroup(object item)
		{
			StaffGroupSummary group = (StaffGroupSummary)item;
			return group.Name;
		}

    	public StaffGroupSummary SelectedGroup
    	{
			get { return _worklistDetail.OwnerGroup; }
			set
			{
				if (!Equals(_worklistDetail.OwnerGroup, value))
				{
					_worklistDetail.OwnerGroup = value;
					this.Modified = true;
					NotifyPropertyChanged("SelectedGroup");
				}
			}
    	}

        [ValidateNotNull]
        public string Name
        {
            get { return _worklistDetail.Name; }
            set
            {
                _worklistDetail.Name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _worklistDetail.Description; }
            set
            {
                _worklistDetail.Description = value;
                this.Modified = true;
            }
        }

    	public bool IsWorklistClassReadOnly
    	{
            get { return _editorMode == WorklistEditorMode.Edit; }
    	}

		[ValidateNotNull]
		public WorklistClassSummary WorklistClass
    	{
			get { return _worklistDetail.WorklistClass; }
			set
			{
				if(!Equals(_worklistDetail.WorklistClass, value))
				{
					_worklistDetail.WorklistClass = value;
					if(_worklistDetail.WorklistClass != null)
					{
						// update settings, but don't save
						WorklistEditorComponentSettings.Default.DefaultWorklistClass = _worklistDetail.WorklistClass.ClassName;
					}
					this.Modified = true;
					NotifyPropertyChanged("WorklistClass");
					NotifyPropertyChanged("WorklistClassDescription");
				}
			}
    	}

		public string FormatWorklistClass(object item)
		{
			WorklistClassSummary summary = (WorklistClassSummary) item;
			return summary.DisplayName;
		}

        public string WorklistClassDescription
        {
            get
            {
				return _worklistDetail.WorklistClass == null ? null : _worklistDetail.WorklistClass.Description;
            }
        }

        public bool AcceptButtonVisible
        {
            get { return _dialogMode; }
        }

        public bool CancelButtonVisible
        {
            get { return _dialogMode; }
        }

        public void Accept()
        {
            Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            Exit(ApplicationComponentExitCode.None);
        }

        #endregion

		protected override void UpdateWorklistClassChoices()
		{
			// blank out the selected worklist class if not in the new set of choices
			if(!this.WorklistClassChoices.Contains(_worklistDetail.WorklistClass))
				_worklistDetail.WorklistClass = null;

			base.UpdateWorklistClassChoices();
		}

		private bool HasGroupAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Worklist.Group); }
		}

		private bool HasPersonalAdminAuthority
		{
			get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Worklist.Personal); }
		}

		private static WorklistClassSummary GetDefaultWorklistClass(List<WorklistClassSummary> worklistClasses, WorklistAdminDetail detail)
		{
			return detail.WorklistClass
				?? CollectionUtils.SelectFirst(worklistClasses,
					delegate(WorklistClassSummary w) { return w.ClassName == WorklistEditorComponentSettings.Default.DefaultWorklistClass; });
		}


	}
}
