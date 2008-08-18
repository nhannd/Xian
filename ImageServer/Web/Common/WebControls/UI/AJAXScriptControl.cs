using System.Web.UI;
using AjaxControlToolkit;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    public class AJAXScriptControl : ScriptUserControl
    {
        public AJAXScriptControl()
            : base(false, HtmlTextWriterTag.Div)
        {
        }
    }
}