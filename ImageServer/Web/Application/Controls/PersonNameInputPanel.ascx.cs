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
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageServer.Web.Common.WebControls;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Controls.PersonNameInputPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class PersonNameInputPanel : System.Web.UI.UserControl
    {
        private PersonName _personName;
        private bool _required = false;
        public PersonName PersonName
        {
            set { _personName = value; }
            get
            {
                string singlebyte =
                    StringUtilities.Combine<string>(new string[]
                                                        {
                                                            PersonLastName.Text,
                                                            PersonGivenName.Text,
                                                            PersonMiddleName.Text,
                                                            PersonTitle.Text,
                                                            PersonSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                singlebyte = singlebyte.TrimEnd('^');

                string ideographicName =
                    StringUtilities.Combine<string>(new string[]
                                                        {
                                                            IdeographicLastName.Text,
                                                            IdeographicGivenName.Text,
                                                            IdeographicMiddleName.Text,
                                                            IdeographicTitle.Text,
                                                            IdeographicSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                ideographicName = ideographicName.TrimEnd('^');

                string phoneticName =
                    StringUtilities.Combine<string>(new string[]
                                                        {
                                                            PhoneticLastName.Text,
                                                            PhoneticGivenName.Text,
                                                            PhoneticMiddleName.Text,
                                                            PhoneticTitle.Text,
                                                            PhoneticSuffix.Text
                                                        }, DicomConstants.DicomSeparator, false);

                phoneticName = phoneticName.TrimEnd('^');


                string dicomName = StringUtilities.Combine<string>(new string[]
                                                        {
                                                            singlebyte,
                                                            ideographicName,
                                                            phoneticName
                                                        }, "=", false);

                dicomName = dicomName.TrimEnd('=');

                return new PersonName(dicomName);
            }
        }

        public bool Required
        {
            get { return _required; }
            set { _required = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }
        

        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PhoneticNameRowIndicator.ImageUrl = ImageServerConstants.ImageURLs.PhoneticName;
            IdeographyNameIndicator.ImageUrl = ImageServerConstants.ImageURLs.IdeographyName;
            ScriptTemplate script =
                new ScriptTemplate(typeof(PersonNameInputPanel).Assembly,
                                   "ClearCanvas.ImageServer.Web.Application.Controls.PersonNameInputPanel.js");
            script.Replace("@@CLIENTID@@", ClientID); 
            script.Replace("@@PHONETIC_ROW_CLIENTID@@", PhoneticRow.ClientID);
            script.Replace("@@IDEOGRAPHY_ROW_CLIENTID@@", IdeographicRow.ClientID);

            
            ShowOtherNameFormatButton.OnClientClick = ClientID + "_ShowOtherNameFormats(); return false;";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script.Script, true);

            PersonGivenNameValidator.IgnoreEmptyValue = !Required;
            PersonLastNameValidator.IgnoreEmptyValue = !Required;
        }

        public override void DataBind()
        {
            base.DataBind();

            if (_personName!=null)
            {
                PersonLastName.Text = _personName.LastName;
                PersonMiddleName.Text = _personName.MiddleName;
                PersonGivenName.Text = _personName.FirstName;
                PersonTitle.Text = _personName.Title;

                if (_personName.Phonetic.IsEmpty)
                {
                    //PhoneticRow.Visible = false;
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    PhoneticLastName.Text = _personName.Phonetic.FamilyName;
                    PhoneticGivenName.Text = _personName.Phonetic.GivenName;
                    PhoneticMiddleName.Text = _personName.Phonetic.MiddleName;
                    PhoneticTitle.Text = _personName.Phonetic.Prefix;
                    PhoneticSuffix.Text = _personName.Phonetic.Suffix;
                }

                if (_personName.Ideographic.IsEmpty)
                {
                    //IdeographicRow.Visible = false;

                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                    IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                }
                else
                {
                    IdeographicLastName.Text = _personName.Ideographic.FamilyName;
                    IdeographicGivenName.Text = _personName.Ideographic.GivenName;
                    IdeographicMiddleName.Text = _personName.Ideographic.MiddleName;
                    IdeographicTitle.Text = _personName.Ideographic.Prefix;
                    IdeographicSuffix.Text = _personName.Ideographic.Suffix;
                }
            }
            else
            {
                // only show the single byte row
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                PhoneticRow.Style.Add(HtmlTextWriterStyle.Display, "none");

                IdeographicRow.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
                IdeographicRow.Style.Add(HtmlTextWriterStyle.Display, "none");
            }
            
        }
    }
}