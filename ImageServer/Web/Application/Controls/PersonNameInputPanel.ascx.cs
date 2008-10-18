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

            PersonGivenNameValidator.Required = Required;
            PersonLastNameValidator.Required = Required;
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