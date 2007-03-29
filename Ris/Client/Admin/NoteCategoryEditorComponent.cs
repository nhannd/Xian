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

        private NoteCategoryDetail _noteCategoryDetail;
        private EntityRef _noteCategoryRef;
        private bool _isNew;

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

        public override void Start()
        {
            try
            {
                if (_isNew)
                {
                    _noteCategoryDetail = new NoteCategoryDetail();
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
                
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public NoteCategoryDetail NoteCategoryDetail
        {
            get { return _noteCategoryDetail; }
            set { _noteCategoryDetail = value; }
        }

        public List<string> SeverityChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_severityChoices); }
        }

        public string Severity
        {
            get { return _noteCategoryDetail.Severity == null ? "" : _noteCategoryDetail.Severity.Value; }
            set
            {
                _noteCategoryDetail.Severity = (value == "") ? null : 
                    CollectionUtils.SelectFirst<EnumValueInfo>(_severityChoices,
                        delegate(EnumValueInfo e) { return e.Value == value; });

                this.Modified = true;
            }
        }

        public string Name
        {
            get { return _noteCategoryDetail.Name; }
            set 
            {
                _noteCategoryDetail.Name = value;
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
                    this.ExitCode = ApplicationComponentExitCode.Normal;
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private void SaveChanges()
        {
            try
            {
                if (_isNew)
                {
                    Platform.GetService<INoteCategoryAdminService>(
                        delegate(INoteCategoryAdminService service)
                        {
                            AddNoteCategoryResponse response = service.AddNoteCategory(new AddNoteCategoryRequest(_noteCategoryDetail));
                            _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        });
                }
                else
                {
                    Platform.GetService<INoteCategoryAdminService>(
                        delegate(INoteCategoryAdminService service)
                        {
                            UpdateNoteCategoryResponse response = service.UpdateNoteCategory(new UpdateNoteCategoryRequest(_noteCategoryRef, _noteCategoryDetail));
                            _noteCategoryRef = response.NoteCategory.NoteCategoryRef;
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
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
    }
}
