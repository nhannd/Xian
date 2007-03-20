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

        public override void Start()
        {
            if (_isNew)
            {
                _modalityDetail = new ModalityDetail();
            }
            else
            {
                try
                {
                    Platform.GetService<IModalityAdminService>(
                        delegate(IModalityAdminService service)
                        {
                            LoadModalityForEditResponse response = service.LoadModalityForEdit(new LoadModalityForEditRequest(_modalityRef));
                            _modalityRef = response.ModalityRef;
                            _modalityDetail = response.ModalityDetail;
                        });
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public ModalityDetail ModalityDetail
        {
            get { return _modalityDetail; }
            set { _modalityDetail = value; }
        }

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
                    if (SaveChanges())
                    {
                        this.ExitCode = ApplicationComponentExitCode.Normal;
                        Host.Exit();
                    }
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }
        }

        private bool SaveChanges()
        {
            if (DuplicateIDExist())
            {
                this.Host.ShowMessageBox(SR.MessageDuplicateModalityID, MessageBoxActions.Ok);
                return false;
            }

            try
            {
                if (_isNew)
                {
                    Platform.GetService<IModalityAdminService>(
                        delegate(IModalityAdminService service)
                        {
                            AddModalityResponse response = service.AddModality(new AddModalityRequest(_modalityDetail));
                            _modalityRef = response.Modality.ModalityRef;
                        });
                }
                else
                {
                    Platform.GetService<IModalityAdminService>(
                        delegate(IModalityAdminService service)
                        {
                            UpdateModalityResponse response = service.UpdateModality(new UpdateModalityRequest(_modalityRef, _modalityDetail));
                            _modalityRef = response.Modality.ModalityRef;
                        });
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return true;
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

        private bool DuplicateIDExist()
        {
            List<ModalitySummary> listModality = new List<ModalitySummary>();

            try
            {
                Platform.GetService<IModalityAdminService>(
                    delegate(IModalityAdminService service)
                    {
                        ListAllModalitiesResponse response = service.ListAllModalities(new ListAllModalitiesRequest(false));
                        listModality = response.Modalities;
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            foreach (ModalitySummary m in listModality)
            {
                if (_isNew)
                {
                    if (m.Id == _modalityDetail.Id)
                        return true;                    
                }
                else
                {
                    if (m.Id == _modalityDetail.Id && (_modalityRef != null && _modalityRef == m.ModalityRef))
                        return true;
                }
            }

            return false;
        }

    }
}
