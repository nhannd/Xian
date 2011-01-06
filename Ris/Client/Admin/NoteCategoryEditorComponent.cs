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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="NoteCategoryEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteCategoryEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteCategoryEditorComponent class
    /// </summary>
    [AssociateView(typeof(NoteCategoryEditorComponentViewExtensionPoint))]
    public class NoteCategoryEditorComponent : ApplicationComponent
    {
        private List<EnumValueInfo> _severityChoices;

        private PatientNoteCategoryDetail _noteCategoryDetail;
        private EntityRef _noteCategoryRef;
        private bool _isNew;

        private PatientNoteCategorySummary _noteCategorySummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategoryEditorComponent()
        {
            _isNew = true;
        }

        public NoteCategoryEditorComponent(EntityRef noteCategoryRef)
        {
            _isNew = false;
            _noteCategoryRef = noteCategoryRef;
        }

        public PatientNoteCategorySummary NoteCategorySummary
        {
            get { return _noteCategorySummary; }
        }

        public override void Start()
        {
            if (_isNew)
            {
                _noteCategoryDetail = new PatientNoteCategoryDetail();
            }
            else
            {
                Platform.GetService<INoteCategoryAdminService>(
                    delegate(INoteCategoryAdminService service)
                    {
                        LoadNoteCategoryForEditResponse response = service.LoadNoteCategoryForEdit(new LoadNoteCategoryForEditRequest(_noteCategoryRef));
                        _noteCategoryRef = response.NoteCategoryRef;
                        _noteCategoryDetail = response.NoteCategoryDetail;
                    });
            }

            Platform.GetService<INoteCategoryAdminService>(
                delegate(INoteCategoryAdminService service)
                {
                    GetNoteCategoryEditFormDataResponse response = service.GetNoteCategoryEditFormData(new GetNoteCategoryEditFormDataRequest());
                    _severityChoices = response.SeverityChoices;

                    if (_isNew && _noteCategoryDetail.Severity == null && response.SeverityChoices.Count > 0)
                    {
                        _noteCategoryDetail.Severity = response.SeverityChoices[0];
                    }
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public PatientNoteCategoryDetail NoteCategoryDetail
        {
            get { return _noteCategoryDetail; }
            set { _noteCategoryDetail = value; }
        }

        #region Presentation Model

        public IList SeverityChoices
        {
            get { return _severityChoices; }
        }

        [ValidateNotNull]
        public EnumValueInfo Severity
        {
            get { return _noteCategoryDetail.Severity; }
            set
            {
            	_noteCategoryDetail.Severity = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Category
        {
            get { return _noteCategoryDetail.Category; }
            set 
            {
                _noteCategoryDetail.Category = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _noteCategoryDetail.Description; }
            set 
            {
                _noteCategoryDetail.Description = value;
                this.Modified = true;
            }
        }

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
                    ExceptionHandler.Report(e, SR.ExceptionSaveNoteCategory, this.Host.DesktopWindow,
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

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<INoteCategoryAdminService>(
                delegate(INoteCategoryAdminService service)
                {
                    if (_isNew)
                    {
                        AddNoteCategoryResponse response = service.AddNoteCategory(new AddNoteCategoryRequest(_noteCategoryDetail));
                        _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        _noteCategorySummary = response.NoteCategory;
                    }
                    else
                    {
                        UpdateNoteCategoryResponse response = service.UpdateNoteCategory(new UpdateNoteCategoryRequest(_noteCategoryDetail));
                        _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        _noteCategorySummary = response.NoteCategory;
                    }
                });
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
