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
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Common.Utilities;
using System.Collections;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="WorklistMultiDetailEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class WorklistMultiDetailEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistMultiDetailEditorComponent class
    /// </summary>
    [AssociateView(typeof(WorklistMultiDetailEditorComponentViewExtensionPoint))]
    public class WorklistMultiDetailEditorComponent : ApplicationComponent
    {
        public class WorklistTableEntry
        {
            private bool _checked;
            private readonly WorklistClassSummary _worklistClass;
            private string _name;
            private string _description;

            public WorklistTableEntry(WorklistClassSummary worklistClass, string name)
            {
                _worklistClass = worklistClass;
                _name = name;
                _checked = true;
                _description = MakeDefaultDescription(worklistClass, name);
            }

            public bool Checked
            {
                get { return _checked; }
                set { _checked = value; }
            }

            public WorklistClassSummary Class
            {
                get { return _worklistClass; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public string Description
            {
                get { return _description; }
                set { _description = value; }
            }
        }



        private readonly List<WorklistClassSummary> _worklistClasses;

        private string[] _categoryChoices;
        private string _selectedCategory;
        private Table<WorklistTableEntry> _worklistTable;
        private WorklistTableEntry _selectedWorklist;
        private string _defaultWorklistName;

        private string _procedureTypeGroupClass;
        private event EventHandler _procedureTypeGroupClassChanged;

        private CrudActionModel _worklistActionModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistMultiDetailEditorComponent(List<WorklistClassSummary> worklistClasses)
        {
            _worklistClasses = worklistClasses;
        }

        public override void Start()
        {
            _worklistTable = new Table<WorklistTableEntry>();
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, bool>("Create",
                delegate(WorklistTableEntry item) { return item.Checked; },
                delegate(WorklistTableEntry item, bool value) { item.Checked = value; }, 0.5f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>("Class",
                delegate(WorklistTableEntry item) { return string.Format("{0} - {1}", item.Class.DisplayName, item.Class.Description); }, 1.0f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>("Name",
                delegate(WorklistTableEntry item) { return item.Name; },
                delegate(WorklistTableEntry item, string value) { item.Name = value; }, 1.0f));
            _worklistTable.Columns.Add(new TableColumn<WorklistTableEntry, string>("Description",
                delegate(WorklistTableEntry item) { return item.Description; },
                delegate(WorklistTableEntry item, string value) { item.Description = value; }, 1.0f));


            _categoryChoices = CollectionUtils.Unique(
                CollectionUtils.Map<WorklistClassSummary, string>(_worklistClasses,
                    delegate(WorklistClassSummary wc) { return wc.CategoryName; })).ToArray();

            if (_categoryChoices.Length > 0)
            {
                _selectedCategory = _categoryChoices[0];

                UpdateWorklistClasses();
            }

            _worklistActionModel = new CrudActionModel(false, true, false);
            _worklistActionModel.Edit.SetClickHandler(EditSelectedWorklist);

            UpdateWorklistActionModel();

            // add validation rule to ensure all worklists that will be created have a name
            this.Validation.Add(new ValidationRule("SelectedWorklist",
                delegate 
                {
                    bool allWorklistsHaveNames = CollectionUtils.TrueForAll(_worklistTable.Items,
                        delegate(WorklistTableEntry item) { return !item.Checked || !string.IsNullOrEmpty(item.Name); });

                    return new ValidationResult(allWorklistsHaveNames, SR.MessageWorklistMustHaveName);
                }));

            base.Start();
        }

        public string ProcedureTypeGroupClass
        {
            get { return _procedureTypeGroupClass; }
            private set
            {
                if(_procedureTypeGroupClass != value)
                {
                    _procedureTypeGroupClass = value;
                    EventsHelper.Fire(_procedureTypeGroupClassChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ProcedureTypeGroupClassChanged
        {
            add { _procedureTypeGroupClassChanged += value; }
            remove { _procedureTypeGroupClassChanged -= value; }
        }

        public List<WorklistTableEntry> WorklistsToCreate
        {
            get
            {
                return CollectionUtils.Select(_worklistTable.Items,
                    delegate(WorklistTableEntry item) { return item.Checked; });
            }
        }

        #region Presentation Model

        public string[] CategoryChoices
        {
            get { return _categoryChoices; }
        }

        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if(value != _selectedCategory)
                {
                    _selectedCategory = value;
                    this.Modified = true;
                    UpdateWorklistClasses();
                    NotifyPropertyChanged("SelectedCategory");
                }
            }
        }

        public string DefaultWorklistName
        {
            get { return _defaultWorklistName; }
            set
            {
                if(value != _defaultWorklistName)
                {
                    UpdateWorklistNamesAndDescriptions(_defaultWorklistName, value);
                    _defaultWorklistName = value;
                    this.Modified = true;
                    NotifyPropertyChanged("DefaultWorklistName");
                }
            }
        }

        public ActionModelNode WorklistActionModel
        {
            get { return _worklistActionModel; }
        }

        public ITable WorklistTable
        {
            get { return _worklistTable; }
        }

        public ISelection SelectedWorklist
        {
            get { return new Selection(_selectedWorklist); }
            set
            {
                WorklistTableEntry item = (WorklistTableEntry) value.Item;
                if(!Equals(item, _selectedWorklist))
                {
                    _selectedWorklist = item;
                    UpdateWorklistActionModel();
                    NotifyPropertyChanged("SelectedWorklist");
                }
            }
        }

        public void EditSelectedWorklist()
        {
            if(_selectedWorklist != null)
            {
                WorklistAdminDetail detail = new WorklistAdminDetail();
                detail.Name = _selectedWorklist.Name;
                detail.Description = _selectedWorklist.Description;
                detail.WorklistClass = _selectedWorklist.Class;

                if(ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow,
                    new WorklistDetailEditorComponent(detail, true),
                    "Edit Worklist") == ApplicationComponentExitCode.Accepted)
                {
                    _selectedWorklist.Name = detail.Name;
                    _selectedWorklist.Description = detail.Description;
                    _worklistTable.Items.NotifyItemUpdated(_selectedWorklist);
                }
            }
        }

        #endregion

        private void UpdateWorklistClasses()
        {
            List<WorklistClassSummary> worklistClassSubset = CollectionUtils.Select(_worklistClasses,
                delegate(WorklistClassSummary wc) { return wc.CategoryName == _selectedCategory; });

            _worklistTable.Items.Clear();
            _worklistTable.Items.AddRange(
                CollectionUtils.Map<WorklistClassSummary, WorklistTableEntry>(worklistClassSubset,
                    delegate (WorklistClassSummary wc) { return new WorklistTableEntry(wc, _defaultWorklistName);}));

            // all classes in subset should have the same procedureTypeGroupClass, so just grab the first one
            this.ProcedureTypeGroupClass = CollectionUtils.FirstElement(worklistClassSubset).ProcedureTypeGroupClassName;
        }

        private void UpdateWorklistNamesAndDescriptions(string oldName, string newName)
        {
            // only update the names and descriptions that the user has not explicitly modified
            foreach (WorklistTableEntry item in _worklistTable.Items)
            {
                if(item.Name == oldName)
                {
                    item.Name = newName;
                }
                if(item.Description == MakeDefaultDescription(item.Class, oldName))
                {
                    item.Description = MakeDefaultDescription(item.Class, newName);
                }
                _worklistTable.Items.NotifyItemUpdated(item);
            }
        }

        private static string MakeDefaultDescription(WorklistClassSummary worklistClass, string name)
        {
            return string.Format("{0} {1}", name, worklistClass.DisplayName);
        }

        private void UpdateWorklistActionModel()
        {
            _worklistActionModel.Edit.Enabled = _selectedWorklist != null;
        }

    }
}
