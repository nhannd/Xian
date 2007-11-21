using System;
using System.Configuration;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Validate if an input control value matches a specified regular expression.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RegularExpressionFieldValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the IP Address. If the input is not an IP address, the IP address
    /// text box will be highlighted.
    /// 
    /// <clearcanvas:RegularExpressionFieldValidator 
    ///                                ID="RegularExpressionFieldValidator1" runat="server"
    ///                                ControlToValidate="IPAddressTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                ValidationExpression="^([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])\.([1-9]?\d|1\d\d|2[0-4]\d|25[0-5])$"
    ///                                ErrorMessage="The IP address is not valid." Display="None">
    /// </clearcanvas:RegularExpressionFieldValidator>
    /// 
    /// </example>
    /// 
    public class RegularExpressionFieldValidator:BaseValidator
    {
        #region Private Members
        private string _evalFunctionName;
        private string _invalidInputBackColor;
        private string _regEx;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets or gets the regular expression to validate the input.
        /// </summary>
        public new string ValidationExpression
        {
            get { return _regEx; }
            set { _regEx = value; }
        }

        /// <summary>
        /// Gets or retrieves the specified background color for the input control when the validation fails.
        /// </summary>
        public string InvalidInputBackColor
        {
            get { return _invalidInputBackColor; }
            set { _invalidInputBackColor = value; }
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            // Register Javascript for client-side validation

            _evalFunctionName = ClientID + "_Evaluation";
             
            string script =
                "<script language='javascript'>" + @"
                    
                        function " + _evalFunctionName + @"()
                        {
                            extenderCtrl =  document.getElementById('" + this.ClientID + @"');                            
                            textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                                          
                            var re = new RegExp('" + ValidationExpression.Replace("\\", "\\\\") + @"');
                            if (textbox.value=='')
                            {
                                result = true;
                            }
                            else if (textbox.value.match(re)) {
                                result = true;
                            } else {
                                result = false;
                            }

                            if (!result)
                            {
                                extenderCtrl.style.visibility='visible';
                                textbox.style.backgroundColor ='" + InvalidInputBackColor + @"';
                            }
                            else
                            {
                                extenderCtrl.style.visibility='hidden';
                                //textbox.style.backgroundColor='" + System.Drawing.ColorTranslator.ToHtml(BackColor) + @"';
                            }

                            //alert('javascript test=' + result);

                            return result;

                        }

                    </script>";

            Page.RegisterClientScriptBlock(ClientID, script);

            base.OnInit(e);
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (RenderUplevel)
            {
                // Add client-side validation function
                writer.AddAttribute("evaluationfunction", _evalFunctionName);
            }
        }

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            bool result = true;
            TextBox input = FindControl(ControlToValidate) as TextBox;
            Regex regex = new Regex(ValidationExpression);
            if (String.IsNullOrEmpty(input.Text) == false)
                result= regex.IsMatch(input.Text);

            return result;
        }

        #endregion Protected Methods
    }
}
