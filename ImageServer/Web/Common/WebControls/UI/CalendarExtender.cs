// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    public class CalendarExtender : AjaxControlToolkit.CalendarExtender
    {
        public CalendarExtender()
        {
            SetPropertyValue("Format", DateTimeFormatter.DefaultDateFormat);
        }
    }
}