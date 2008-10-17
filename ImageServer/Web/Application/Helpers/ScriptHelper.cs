using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    public class ScriptHelper
    {
        public static string ClearDate(string textBoxID, string calendarExtenderID)
        {
            return "document.getElementById('" + textBoxID + "').value='';" +
                         "$find('" + calendarExtenderID + "').set_selectedDate(null);" +
                         "return false;";
        }
    }
}
