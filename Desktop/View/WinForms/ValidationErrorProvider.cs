using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class ValidationErrorProvider : ErrorProvider
    {
        public ValidationErrorProvider()
        {
            this.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        public new object DataSource
        {
            get { return base.DataSource; }
            set
            {
                if (value != base.DataSource)
                {
                    if (base.DataSource is IApplicationComponent)
                    {
                        IApplicationComponent c = base.DataSource as IApplicationComponent;
                        c.ValidationVisibleChanged -= ApplicationComponent_ShowValidationErrorsChanged;
                    }

                    base.DataSource = value;

                    if (value is IApplicationComponent)
                    {
                        IApplicationComponent c = value as IApplicationComponent;
                        c.ValidationVisibleChanged += ApplicationComponent_ShowValidationErrorsChanged;
                    }
                }
            }
        }

        private void ApplicationComponent_ShowValidationErrorsChanged(object sender, EventArgs e)
        {
            this.UpdateBinding();
        }
    }
}
