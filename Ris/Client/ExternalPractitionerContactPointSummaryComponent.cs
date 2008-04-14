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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerContactPointSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerContactPointSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerContactPointSummaryComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerContactPointSummaryComponentViewExtensionPoint))]
    public class ExternalPractitionerContactPointSummaryComponent : ApplicationComponent
    {
        private ExternalPractitionerContactPointDetail _selectedContactPoint;
        private readonly Table<ExternalPractitionerContactPointDetail> _contactPoints;

        private readonly CrudActionModel _actionModel;

        private readonly bool _dialogMode;

        private readonly IList<EnumValueInfo> _addressTypeChoices;
        private readonly IList<EnumValueInfo> _phoneTypeChoices;
        private readonly IList<EnumValueInfo> _resultCommunicationModeChoices;


        /// <summary>
        /// Constructor for editing. Set the <see cref="Subject"/> property before starting.
        /// </summary>
        public ExternalPractitionerContactPointSummaryComponent(IList<EnumValueInfo> addressTypeChoices, IList<EnumValueInfo> phoneTypeChoices, IList<EnumValueInfo> resultCommunicationModeChoices)
            :this(false)
        {
            _addressTypeChoices = addressTypeChoices;
            _phoneTypeChoices = phoneTypeChoices;
            _resultCommunicationModeChoices = resultCommunicationModeChoices;
        }

        /// <summary>
        /// Constructor for read-only selection. Set the <see cref="Subject"/> property before starting.
        /// </summary>
        public ExternalPractitionerContactPointSummaryComponent()
            :this(true)
        {
            _addressTypeChoices = new List<EnumValueInfo>();
            _phoneTypeChoices = new List<EnumValueInfo>();
            _resultCommunicationModeChoices = new List<EnumValueInfo>();
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="dialogMode">Indicates whether the component will be shown in a dialog box or not</param>
        private ExternalPractitionerContactPointSummaryComponent(bool dialogMode)
        {
            _dialogMode = dialogMode;
            _contactPoints = new Table<ExternalPractitionerContactPointDetail>();
            _contactPoints.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Name",
               delegate(ExternalPractitionerContactPointDetail cp) { return cp.Name; },
               1.0f));

            _contactPoints.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Description",
                delegate(ExternalPractitionerContactPointDetail cp) { return cp.Description; },
                1.0f));

            _contactPoints.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, bool>("Default",
                delegate(ExternalPractitionerContactPointDetail cp) { return cp.IsDefaultContactPoint; },
                delegate(ExternalPractitionerContactPointDetail cp, bool value)
                {
                    MakeDefaultContactPoint(cp);
                },
                1.0f));

            // TODO implement delete action, which should de-activate the contact point (can't delete it)
            _actionModel = new CrudActionModel(true, true, false);
            _actionModel.Add.SetClickHandler(AddContactPoint);
            _actionModel.Edit.SetClickHandler(UpdateSelectedContactPoint);
        }

        public IItemCollection<ExternalPractitionerContactPointDetail> Subject
        {
            get { return _contactPoints.Items; }
        }


        public override void Start()
        {

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        public bool ShowAcceptCancelButtons
        {
            get { return _dialogMode; }
        }

        public ITable ContactPoints
        {
            get { return _contactPoints; }
        }

        public ActionModelNode ContactPointActionModel
        {
            get { return _actionModel; }
        }

        public ISelection SelectedContactPoint
        {
            get { return _selectedContactPoint == null ? Selection.Empty : new Selection(_selectedContactPoint); }
            set
            {
                ExternalPractitionerContactPointDetail item = (ExternalPractitionerContactPointDetail) value.Item;
                if(item != _selectedContactPoint)
                {
                    _selectedContactPoint = (ExternalPractitionerContactPointDetail)value.Item;
                    PractitionerSelectionChanged();
                }
            }
        }

        public void AddContactPoint()
        {
            try
            {
                ExternalPractitionerContactPointDetail contactPoint = new ExternalPractitionerContactPointDetail();
                contactPoint.PreferredResultCommunicationMode = _resultCommunicationModeChoices.Count > 0 ? _resultCommunicationModeChoices[0] : null;
                ExternalPractitionerContactPointEditorComponent editor = new ExternalPractitionerContactPointEditorComponent(contactPoint, _addressTypeChoices, _phoneTypeChoices, _resultCommunicationModeChoices);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, "Add Contact Point");
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _contactPoints.Items.Add(contactPoint);
                    _selectedContactPoint = contactPoint;
                    NotifyPropertyChanged("SelectedContactPoint");

                    // if item was made default, then make sure no other items are also set as default
                    if(contactPoint.IsDefaultContactPoint)
                        MakeDefaultContactPoint(contactPoint);
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedContactPoint()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_selectedContactPoint == null) return;

                // clone item in case the modal edit dialog is cancelled
                ExternalPractitionerContactPointDetail contactPoint = (ExternalPractitionerContactPointDetail)_selectedContactPoint.Clone();
                ExternalPractitionerContactPointEditorComponent editor = new ExternalPractitionerContactPointEditorComponent(contactPoint, _addressTypeChoices, _phoneTypeChoices, _resultCommunicationModeChoices);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Host.DesktopWindow, editor, string.Format("Edit Contact Point '{0}'", contactPoint.Name));
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // replace row with updated item
                    int i =_contactPoints.Items.FindIndex(
                        delegate(ExternalPractitionerContactPointDetail cp) { return cp == _selectedContactPoint; });
                    _contactPoints.Items[i] = contactPoint;
                    _selectedContactPoint = contactPoint;
                    NotifyPropertyChanged("SelectedContactPoint");

                    // if item was made default, then make sure no other items are also set as default
                    if (contactPoint.IsDefaultContactPoint)
                        MakeDefaultContactPoint(contactPoint);

                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DoubleClickSelectedContactPoint()
        {
            // double-click behaviour is different depending on whether we're running as a dialog box or not
            if (_dialogMode)
                Accept();
            else
                UpdateSelectedContactPoint();
        }

        public bool AcceptEnabled
        {
            get { return _selectedContactPoint != null; }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Accepted;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }

        #endregion

        private void PractitionerSelectionChanged()
        {
            _actionModel.Edit.Enabled = (_selectedContactPoint != null);
            _actionModel.Delete.Enabled = (_selectedContactPoint != null);
        }

        private void MakeDefaultContactPoint(ExternalPractitionerContactPointDetail cp)
        {
            foreach (ExternalPractitionerContactPointDetail item in _contactPoints.Items)
            {
                item.IsDefaultContactPoint = (item == cp);
                _contactPoints.Items.NotifyItemUpdated(item);
            }
        }

    }
}
