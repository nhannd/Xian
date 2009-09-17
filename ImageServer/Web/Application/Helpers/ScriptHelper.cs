#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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

        public static string CheckDateRange(string fromDateTextBoxID, string toDateTextBoxID, string textBoxID, string calendarExtenderID, string message)
        {
            return
                "CheckDateRange(document.getElementById('" + fromDateTextBoxID + "').value, document.getElementById('" +
                toDateTextBoxID + "').value, '" + textBoxID + "' , '" + calendarExtenderID + "' , '" + message + "');";
        }

        public static string PopulateDefaultFromTime(string fromTimeTextBoxID)
        {
            return "if(document.getElementById('" + fromTimeTextBoxID + "').value == '') { document.getElementById('" + fromTimeTextBoxID + "').value = '00:00:00.000'; }";
        }

        public static string PopulateDefaultToTime(string toTimeTextBoxID)
        {
            return "if(document.getElementById('" + toTimeTextBoxID + "').value == '') { document.getElementById('" + toTimeTextBoxID + "').value = '23:59:59.999'; }";
        }

    }
}
