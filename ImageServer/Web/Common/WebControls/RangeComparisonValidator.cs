using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Validate if the value in the number input control is in a specified range and that it is 
    /// less than or greater than the value of another input control.
    /// </summary>
    /// <remarks>
    /// This control has different behaviour than standard ASP.NET <seealso cref="RangeValidator"/>.
    /// Developers can optionally specify the background color for the input control if the 
    /// validation fails.  Also, a comparison can be done against another control to make sure the value
    /// of the control is less than or greater than the other control.
    /// </remarks>
    /// <example>
    /// The following block adds validation for the High Watermark. If the input is not within 1 and 99
    /// the High Watermark text box will be highlighted.
    /// 
    /// <clearcanvas:RangeComparisonValidator 
    ///                        ID="RangeValidator1" runat="server"
    ///                        ControlToValidate="HighWatermark"
    ///                        ControlToCompare="LowWatermark"
    ///                        GreaterThan="true"
    ///                        InvalidInputBackColor="#FAFFB5"
    ///                        ValidationGroup="vg1" 
    ///                        MinValue="1"
    ///                        MaxValue="99"
    ///                        ErrorMessage="The High Watermark must be between 1 and 99 and greater than the low watermark.">
    /// </clearcanvas:RangeValidator>
    /// </example>
    public class RangeComparisonValidator : BaseValidator
    {
        #region Private Members
        private int _min;
        private int _max;
        private string _comparisonControlId;
        private bool _greaterThan;
        #endregion Private Members

        #region Public Properties


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

        public string ControlToCompare
        {
            get { return _comparisonControlId; }
            set { _comparisonControlId = value; }
        }

        public bool GreaterThan
        {
            get { return _greaterThan; }
            set { _greaterThan = value; }
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            // Register Javascript for client-side validation

            string comparison = GreaterThan ? ">=" : "<=";
            string script =
                "<script language='javascript'>" + @"
                    function " + EvalFunctionName + @"()
                    {
                        extenderCtrl =  document.getElementById('" + this.ClientID + @"');                            
                        textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                        compareCtrl = document.getElementById('" + GetControlRenderID(ControlToCompare) + @"');
                        helpCtrl = document.getElementById('" + GetControlRenderID(PopupHelpControlID) + @"');
 
                        result = true;
                        if (textbox.value!=null && textbox.value!='' && compareCtrl!=null && compareCtrl.value!='')
                        {
                            compareValue = parseInt(compareCtrl.value);
                            controlValue = parseInt(textbox.value);
                            result = controlValue >= " + MinValue + @" && controlValue<= " + MaxValue + @" && controlValue " + comparison + @" compareValue;
                        }   
                        else
                        {
                            result = false;
                        }
                        
                        if (!result)
                        {
                            extenderCtrl.style.visibility='visible';
                            if (helpCtrl!=null){
                                helpCtrl.style.visibility='visible';
                                helpCtrl.alt='" + ErrorMessage + @"';
                            }
                            textbox.style.backgroundColor ='" + InvalidInputBackColor + @"';
                        }
                        else
                        {
                            //if (helpCtrl!=null)
                            //    helpCtrl.style.visibility='hidden';
                            //extenderCtrl.style.visibility='hidden';
                            //textbox.style.backgroundColor='" + ColorTranslator.ToHtml(BackColor) + @"';
                        }
                        //alert('javascript test=' + result);
                        return result;
                    }

                </script>";

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script);

            base.OnInit(e);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (RenderUplevel)
            {
                // Add client-side validation function
                writer.AddAttribute("evaluationfunction", EvalFunctionName);
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
            if (input != null)
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
