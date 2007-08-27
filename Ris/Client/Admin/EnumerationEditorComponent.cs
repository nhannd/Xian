using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client.Admin
{
    /// <summary>
    /// Extension point for views onto <see cref="EnumerationEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EnumerationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EnumerationEditorComponent class
    /// </summary>
    [AssociateView(typeof(EnumerationEditorComponentViewExtensionPoint))]
    public class EnumerationEditorComponent : ApplicationComponent
    {
        private EnumValueInfo _enumValue;

        private bool _isNew;
        private string _enumerationClassName;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationEditorComponent(string enumerationClassName)
        {
            _isNew = true;
            _enumerationClassName = enumerationClassName;
            _enumValue = new EnumValueInfo();
        }

        public EnumerationEditorComponent(string enumerationName, EnumValueInfo value)
        {
            _isNew = false;
            _enumerationClassName = enumerationName;
            _enumValue = value;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public EnumValueInfo EnumValue
        {
            get { return _enumValue; }
        }

        #region Presentation Model

        public bool IsCodeReadOnly
        {
            get { return !_isNew; }
        }

        [ValidateNotNull]
        [ValidateRegex(@"^\w+$", Message = "MessageEnumCodeContainsInvalidChars", AllowNull = true)]
        public string Code
        {
            get { return _enumValue.Code; }
            set
            {
                _enumValue.Code = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string DisplayValue
        {
            get { return _enumValue.Value; }
            set
            {
                _enumValue.Value = value;
                this.Modified = true;
            }
        }

        public string Description
        {
            get { return _enumValue.Description; }
            set
            {
                _enumValue.Description = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            try
            {
                Platform.GetService<IEnumerationAdminService>(
                    delegate(IEnumerationAdminService service)
                    {
                        if (_isNew)
                        {
                            service.AddValue(new AddValueRequest(_enumerationClassName, _enumValue));
                        }
                        else
                        {
                            service.EditValue(new EditValueRequest(_enumerationClassName, _enumValue));
                        }

                    });

                Exit(ApplicationComponentExitCode.Normal);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, _isNew ? SR.ExceptionEnumValueAdd : SR.ExceptionEnumValueUpdate, this.Host.DesktopWindow,
                    delegate()
                    {
                        Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        public void Cancel()
        {
            Exit(ApplicationComponentExitCode.Cancelled);
        }

        #endregion
    }
}
