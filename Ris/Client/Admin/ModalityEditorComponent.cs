using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ModalityEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ModalityEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ModalityEditorComponent class
    /// </summary>
    [AssociateView(typeof(ModalityEditorComponentViewExtensionPoint))]
    public class ModalityEditorComponent : ApplicationComponent
    {
        private ModalityDetail _modalityDetail;
        private EntityRef _modalityRef;
        private bool _isNew;

        private ModalitySummary _modalitySummary;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalityEditorComponent()
        {
            _isNew = true;
        }

        public ModalityEditorComponent(EntityRef modalityRef)
        {
            _isNew = false;
            _modalityRef = modalityRef;
        }

        /// <summary>
        /// Gets the summary object that is returned from the add/edit operation
        /// </summary>
        public ModalitySummary ModalitySummary
        {
            get
            {
                return _modalitySummary;
            }
        }

        public override void Start()
        {
            if (_isNew)
            {
                _modalityDetail = new ModalityDetail();
            }
            else
            {
                Platform.GetService<IModalityAdminService>(
                    delegate(IModalityAdminService service)
                    {
                        LoadModalityForEditResponse response = service.LoadModalityForEdit(new LoadModalityForEditRequest(_modalityRef));
                        _modalityRef = response.ModalityRef;
                        _modalityDetail = response.ModalityDetail;
                    });
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ModalityDetail ModalityDetail
        {
            get { return _modalityDetail; }
            set { _modalityDetail = value; }
        }

        #region Presentation Model

        public string ID
        {
            get { return _modalityDetail.Id; }
            set
            {
                _modalityDetail.Id = value;
                this.Modified = true;
            }
        }

        public string Name
        {
            get { return _modalityDetail.Name; }
            set
            {
                _modalityDetail.Name = value;
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
                    Host.Exit();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionSaveModality, this.Host.DesktopWindow,
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
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<IModalityAdminService>(
                delegate(IModalityAdminService service)
                {
                    if (_isNew)
                    {
                        AddModalityResponse response = service.AddModality(new AddModalityRequest(_modalityDetail));
                        _modalityRef = response.Modality.ModalityRef;
                        _modalitySummary = response.Modality;
                    }
                    else
                    {
                        UpdateModalityResponse response = service.UpdateModality(new UpdateModalityRequest(_modalityRef, _modalityDetail));
                        _modalityRef = response.Modality.ModalityRef;
                        _modalitySummary = response.Modality;
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
