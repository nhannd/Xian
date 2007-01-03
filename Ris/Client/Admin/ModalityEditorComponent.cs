using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;

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
        private Modality _modality;
        private EntityRef<Modality> _modalityRef;
        private IModalityAdminService _modalityAdminService;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModalityEditorComponent()
        {
            _isNew = true;
        }

        public ModalityEditorComponent(EntityRef<Modality> modalityRef)
        {
            _isNew = false;
            _modalityRef = modalityRef;
        }

        public override void Start()
        {
            _modalityAdminService = ApplicationContext.GetService<IModalityAdminService>();

            try
            {
                if (_isNew)
                    _modality = new Modality();
                else
                    _modality = _modalityAdminService.LoadModality(_modalityRef);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string ID
        {
            get { return _modality.Id; }
            set
            {
                _modality.Id = value;
                this.Modified = true;
            }
        }

        public string Name
        {
            get { return _modality.Name; }
            set
            {
                _modality.Name = value;
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
                catch (ConcurrencyException e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionConcurrencyModalityNotSaved, this.Host.DesktopWindow);
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

            if (_isNew)
            {
                _modalityAdminService.AddModality(_modality);
                _modalityRef = new EntityRef<Modality>(_modality);
            }
            else
            {
                _modalityAdminService.UpdateModality(_modality);
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
            IList<Modality> listModality = _modalityAdminService.GetAllModalities();
            foreach (Modality m in listModality)
            {
                if (_isNew)
                {
                    if (m.Id == _modality.Id)
                        return true;                    
                }
                else
                {
                    if (m.Id == _modality.Id && (_modalityRef != null && _modalityRef.RefersTo(m) == false))
                        return true;
                }
            }

            return false;
        }

    }
}
