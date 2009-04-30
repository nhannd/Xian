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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Represents a label control which displays a person name on a Web page.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="PersonNameLabel"/> to display a person name on a web page. The person's name to be displayed is set through 
    /// the <see cref="PersonName"/> property. <see cref="PersonNameLabel"/> accepts person name in dicom format. To indicate 
    /// Dicom formatted name, set <see cref="PersonNameType"/> to <see cref="NameType.Dicom"/>. In such case, the The person name will be rendered 
    /// in a human readable format (e.g., "Smith, John" )
    /// 
    ///  
    /// is not set, the date/time format will set to one of the followings, in the listed order:
    /// 
    /// - The date and time formats specified in the web configuration. 
    /// - The default date/time formats for the UI culture specified in <globalization>
    /// - The default date/time format for the region setting of the system. For eg, for English (Canada) region, 
    /// the default Long date format is "MMMM dd, yyyy". For US, the default date format is "DDD, MMMM dd, yyyy"
    /// 
    /// </remarks>
    /// <example>
    /// </example>
    [DefaultProperty("Value")]
    [ToolboxData("<{0}:PatientNameLabel runat=server></{0}:PatientNameLabel>")]
    public class PersonNameLabel : Label
    {

        public enum NameType
        {
            Normal,
            Dicom
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string PersonName
        {
            get
            {
                return ViewState["PersonName"] as string;
            }
            set
            {
                ViewState["PersonName"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public NameType PersonNameType
        {
            get
            {
                return (NameType)ViewState["PersonNameType"];
            }
            set
            {
                ViewState["PersonNameType"] = value;
            }
        }


        protected override void RenderContents(HtmlTextWriter writer)
        {
            string text = GetRenderText();
            if (!String.IsNullOrEmpty(text))
                writer.Write(text);
        }
        
        protected string GetRenderText()
        {
            if (String.IsNullOrEmpty(Text))
                return GetRenderedPatientName();
            else
                return String.Format(Text, GetRenderedPatientName());
        }

        protected virtual string GetRenderedPatientName()
        {
            string name = PersonName;

            switch (PersonNameType)
            {
                case NameType.Normal:
                    return name;

                case NameType.Dicom:
                    PersonName pn = new PersonName(name);
                    return NameFormatter.Format(name, UISettings.Default.NameFormat);

                default:
                    return name; // no formatting
            }
        }
    }
}
