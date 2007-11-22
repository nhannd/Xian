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
    /// Validate if the value in the number input control is in a specified range.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RangeValidator"/>.
    /// Developers can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the Port number. If the input is not within 0 and 32000
    /// the Port text box will be highlighted.
    /// 
    /// <clearcanvas:RangeValidator 
    ///                                ID="RangeValidator1" runat="server"
    ///                                ControlToValidate="PortTextBox"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                MinValue="0"
    ///                                MaxValue="32000"
    ///                                ErrorMessage="The Port number is not valid.">
    /// </clearcanvas:RangeValidator>
    /// 
    /// </example>
    /// 
    public class RangeValidator:BaseValidator
    {
        #region Private Members
        private string _evalFunctionName;
        private string _invalidInputBackColor;
        private int _min;
        private int _max;
        #endregion Private Members

        #region Public Properties


        /// <summary>
        /// Gets or retrieves the specified background color for the input control when the validation fails.
        /// </summary>
        public string InvalidInputBackColor
        {
            get { return _invalidInputBackColor; }
            set { _invalidInputBackColor = value; }
        }

        /// <summary>
        /// Sets or gets the minimum acceptable value.
        /// </summary>
        public int MinValue
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Sets or gets the maximum acceptable value.
        /// </summary>
        public int MaxValue
        {
            get { return _max; }
            set { _max = value; }
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
                                    
                            result = true;
                            if (textbox.value!=null && textbox.value!='')
                            {
                                portValue = parseInt(textbox.value);
                                result = portValue >= " + _min + @" && portValue<= " + _max + @";
                            }   
                            else
                            {
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
            bool result = false;
            TextBox input = FindControl(ControlToValidate) as TextBox;
            if (String.IsNullOrEmpty(input.Text) == false)
            {
                Int32 value;
                result = Int32.TryParse(input.Text, out value);
            }
            return result;
        }

        #endregion Protected Methods
    }
}
