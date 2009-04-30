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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.UI
{
    /// <summary>
    /// Represents a text label control which displays a DICOM DA value on a Web page.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the <see cref="DALabel"/> to display a date on a web page for a DICOM DA value. Unlike the DA value, the time displayed on the web page will
    /// be more user-friendly. The date to be displayed (DA value) is set through  the <see cref="Value"/> property. The format of the time can be set through the <see cref="DateTimeLabel.Format"/> property. 
    /// If <see cref="DateTimeLabel.Format"/>  is not set, the date format will set to one of the followings, in the listed order:
    /// </para>
    /// 
    /// - The date format specified in the web configuration. 
    /// - The default date format for the UI culture specified in <globalization>
    /// - The default date format for the region and langugage of the system.
    /// 
    /// <para>
    /// If the DA value is empty or null, the text specified in <see cref="EmptyValueText"/> will be displayed. If the <see cref="Value"/> is an invalid DA value,
    /// the text specified in <see cref="InvalidValueText"/> will be displayed.
    /// </para>
    /// 
    /// <seealso cref="DateTimeLabel"/>
    /// 
    /// </remarks>
    /// <example>
    /// </example>
    /// 
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DALabel runat=server></{0}:DALabel>")]
    public class DALabel : DateTimeLabel
    {
        /// <summary>
        /// The DA value for the date to be displayed on the web page
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public new string Value
        {
            get
            {
                return ViewState["ValueDA"] as string;
            }

            set
            {
                ViewState["ValueDA"] = value;
            }
        }

        /// <summary>
        /// The text to be displayed on the web page when the DA value is empty
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]        
        public string EmptyValueText
        {
            get
            {
                return ViewState["EmptyValueText"] as string;
            }

            set
            {
                ViewState["EmptyValueText"] = value;
            }
        }

        /// <summary>
        /// The text to be displayed on the web page when the DA value is invalid
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string InvalidValueText
        {
            get
            {
                return ViewState["InvalidValueText"] as string;
            }

            set
            {
                ViewState["InvalidValueText"] = value;
            }
        }


        protected override string GetRenderedDateTimeText()
        {
            if (String.IsNullOrEmpty(Value))
                return EmptyValueText;

            DateTime? dt = DateParser.Parse(Value);

            if (dt != null)
            {
                if (String.IsNullOrEmpty(Format))
                    return DateTimeFormatter.Format(dt.Value, DateTimeFormatter.Style.Date);
                else
                    return dt.Value.ToString(Format);
            }
            else
                return null;
           
        }

       
    }
}
