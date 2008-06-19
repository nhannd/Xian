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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Enumerations", "Launch")]
	[ActionPermission("launch", AuthorityTokens.Admin.Data.Enumeration)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class EnumerationAdminTool : Tool<IDesktopToolContext>
    {
        private Workspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    EnumerationSummaryComponent component = new EnumerationSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleEnumerationAdmin);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }




    /// <summary>
    /// Extension point for views onto <see cref="EnumerationSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EnumerationSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EnumerationSummaryComponent class
    /// </summary>
    [AssociateView(typeof(EnumerationSummaryComponentViewExtensionPoint))]
    public class EnumerationSummaryComponent : ApplicationComponent
    {
        private List<EnumerationSummary> _enumerations;
        private EnumerationSummary _selectedEnumeration;

        private Table<EnumValueInfo> _enumValues;
        private EnumValueInfo _selectedEnumValue;

        private CrudActionModel _crudActionModel;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationSummaryComponent()
        {
        }

        public override void Start()
        {
            _enumValues = new Table<EnumValueInfo>();
            _enumValues.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumCode,
                delegate(EnumValueInfo info) { return info.Code; }, 0.4F));
            _enumValues.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumValue,
                delegate(EnumValueInfo info) { return info.Value; }, 1.0F));
            _enumValues.Columns.Add(new TableColumn<EnumValueInfo, string>(SR.ColumnEnumDescription,
                delegate(EnumValueInfo info) { return info.Description; }, 1.5F));

            _crudActionModel = new CrudActionModel();
            _crudActionModel.Add.SetClickHandler(AddValue);
			_crudActionModel.Add.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);
            _crudActionModel.Edit.SetClickHandler(EditValue);
			_crudActionModel.Edit.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);
			_crudActionModel.Delete.SetClickHandler(RemoveValue);
			_crudActionModel.Delete.SetPermissibility(AuthorityTokens.Admin.Data.Enumeration);

            Platform.GetService<IEnumerationAdminService>(
                delegate(IEnumerationAdminService service)
                {
                    ListEnumerationsResponse response = service.ListEnumerations(new ListEnumerationsRequest());
                    _enumerations = response.Enumerations;
                    _enumerations.Sort(delegate(EnumerationSummary x, EnumerationSummary y) { return x.DisplayName.CompareTo(y.DisplayName); });
                });

            _selectedEnumeration = CollectionUtils.FirstElement(_enumerations);

            LoadEnumerationValues();
            UpdateCrudEnablement();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public ActionModelNode CrudActionModel
        {
            get { return _crudActionModel; }
        }

        public List<string> EnumerationChoices
        {
            get
            {
                return CollectionUtils.Map<EnumerationSummary, string, List<string>>(_enumerations,
                    delegate(EnumerationSummary s) { return s.DisplayName; });
            }
        }

        public string SelectedEnumeration
        {
            get { return _selectedEnumeration.DisplayName; }
            set
            {
                EnumerationSummary summary = CollectionUtils.SelectFirst<EnumerationSummary>(_enumerations,
                    delegate(EnumerationSummary s) { return s.DisplayName == value; });

                if (_selectedEnumeration != summary)
                {
                    _selectedEnumeration = summary;

                    LoadEnumerationValues();
                    UpdateCrudEnablement();

                    NotifyPropertyChanged("SelectedEnumeration");
                    NotifyPropertyChanged("SelectedEnumerationClassName");
                }
            }
        }

        public string SelectedEnumerationClassName
        {
            get { return _selectedEnumeration == null ? null : _selectedEnumeration.AssemblyQualifiedClassName; }
        }

        public ITable EnumerationValues
        {
            get { return _enumValues; }
        }

        public ISelection SelectedEnumerationValue
        {
            get { return new Selection(_selectedEnumValue); }
            set
            {
                EnumValueInfo val = (EnumValueInfo)value.Item;
                if (val != _selectedEnumValue)
                {
                    _selectedEnumValue = val;

                    UpdateCrudEnablement();
                    NotifyPropertyChanged("SelectedEnumerationValue");
                }
            }
        }

        public void EnumerationValueDoubleClicked()
        {
            EditValue();
        }
        
        #endregion

        private void LoadEnumerationValues()
        {
            _enumValues.Items.Clear();
            if (_selectedEnumeration != null)
            {
                Platform.GetService<IEnumerationAdminService>(
                    delegate(IEnumerationAdminService service)
                    {
                        ListEnumerationValuesResponse response = 
                            service.ListEnumerationValues(new ListEnumerationValuesRequest(_selectedEnumeration.AssemblyQualifiedClassName));

                        _enumValues.Items.AddRange(response.Values);
                    });
            }
        }

        private void AddValue()
        {
            try
            {
                EnumerationEditorComponent component = new EnumerationEditorComponent(
                    _selectedEnumeration.AssemblyQualifiedClassName,
                    _enumValues.Items);
                ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleEnumAddValue);
                if(result == ApplicationComponentExitCode.Accepted)
                {
                    // refresh entire table
                    LoadEnumerationValues();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void EditValue()
        {
            try
            {
                EnumerationEditorComponent component = new EnumerationEditorComponent(
                    _selectedEnumeration.AssemblyQualifiedClassName,
                    (EnumValueInfo)_selectedEnumValue.Clone(),
                    _enumValues.Items);
                ApplicationComponentExitCode result = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitleEnumEditValue + " - " + _selectedEnumValue.Code);
                if (result == ApplicationComponentExitCode.Accepted)
                {
                    // refresh entire table
                    LoadEnumerationValues();
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        private void RemoveValue()
        {
            DialogBoxAction result = this.Host.ShowMessageBox(SR.MessageEnumConfirmDelete, MessageBoxActions.YesNo);
            if (result == DialogBoxAction.Yes)
            {
                try
                {
                    Platform.GetService<IEnumerationAdminService>(
                        delegate(IEnumerationAdminService service)
                        {
                            EnumValueInfo valueToDelete = _selectedEnumValue;
                            service.RemoveValue(new RemoveValueRequest(_selectedEnumeration.AssemblyQualifiedClassName, valueToDelete));

                            _enumValues.Items.Remove(valueToDelete);
                        });
                    _selectedEnumValue = null;

                    UpdateCrudEnablement();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionEnumValueDelete, this.Host.DesktopWindow);
                }
            }
        }

        private void UpdateCrudEnablement()
        {
            _crudActionModel.Add.Enabled = (_selectedEnumeration != null && _selectedEnumeration.CanAddRemoveValues);
            _crudActionModel.Delete.Enabled = (_selectedEnumeration != null && _selectedEnumeration.CanAddRemoveValues && _selectedEnumValue != null);
            _crudActionModel.Edit.Enabled = (_selectedEnumeration != null && _selectedEnumValue != null);
        }

    }
}
