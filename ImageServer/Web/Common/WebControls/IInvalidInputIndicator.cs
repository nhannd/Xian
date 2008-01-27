using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    public interface IInvalidInputIndicator
    {
        Control Container
        { get;
        }

        Label TooltipLabel
        {
            get;
        }

        void AttachValidator(BaseValidator validator);

        void Show();
        void Hide();

    }
}
