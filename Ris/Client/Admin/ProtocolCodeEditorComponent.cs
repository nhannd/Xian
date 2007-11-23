using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="ProtocolCodeEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProtocolCodeEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProtocolCodeEditorComponent class
    /// </summary>
    [AssociateView(typeof(ProtocolCodeEditorComponentViewExtensionPoint))]
    public class ProtocolCodeEditorComponent : ApplicationComponent
    {
        #region Private fields

        private string _name = "";
        private string _description = "";
        private ProtocolCodeDetail _protocolCodeDetail;

        #endregion

        #region Presentation Model

        public ProtocolCodeDetail ProtocolCode
        {
            get { return _protocolCodeDetail; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
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
                    ExceptionHandler.Report(e, SR.ExceptionSaveProtocolCode, this.Host.DesktopWindow,
                        delegate()
                        {
                            this.ExitCode = ApplicationComponentExitCode.Error;
                            this.Host.Exit();
                        });
                }
            }
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void SaveChanges()
        {
            Platform.GetService<IProtocolAdminService>(
                delegate(IProtocolAdminService service)
                {
                    AddProtocolCodeRequest request = new AddProtocolCodeRequest(_name, _description);
                    AddProtocolCodeResponse response = service.AddProtocolCode(request);

                    _protocolCodeDetail = response.Detail;
                });
        }
    }
}
