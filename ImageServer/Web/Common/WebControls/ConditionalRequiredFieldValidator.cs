using System;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    /// <summary>
    /// Validate a required Web UI input control containing value based on the state of another checkbox control.
    /// </summary>
    /// <remarks>
    /// This control has slightly different behaviour than standard ASP.NET <seealso cref="RequiredFieldValidator"/>.
    /// Users can use this control for required field validation based on state of a checkbox on the UI. When the 
    /// condition is satisfied, the control will validate the input field contains a value. Developers 
    /// can optionally specify the background color for the input control if the validation fails.
    /// </remarks>
    /// 
    /// <example>
    /// The following block adds validation for the SIN if the citizen checkbox is checked:
    /// 
    /// <clearcanvas:ConditionalRequiredFieldValidator 
    ///                                ID="RequiredFieldValidator2" runat="server" 
    ///                                ControlToValidate="SINTextBox"
    ///                                ConditionalCheckBoxID="IsCitizenCheckedBox" 
    ///                                RequiredWhenChecked="true"
    ///                                InvalidInputBackColor="#FAFFB5"
    ///                                ValidationGroup="vg1" 
    ///                                EnableClientScript="true"
    ///                                ErrorMessage="SIN is required for citizen!!">
    /// </clearcanvas:ConditionalRequiredFieldValidator>
    /// 
    /// </example>
    /// 
    public class ConditionalRequiredFieldValidator: Sample.Web.UI.Compatibility.BaseValidator
    {
        #region Private Members
        // the control, if checked, "enables" this validation control
        private string _conditionalCheckBoxID;
        // specify when the validation should be enabled: when the condition control is checked or unchecked
        private bool    _requiredWhenChecked;
        // the background color of the input when the validation fails.
        private string _invalidInputBackColor;
        
        private string _evalFunctionName;

        #endregion Private members

        #region Public Properties
        /// <summary>
        /// Sets or gets the ID of the condition control.
        /// </summary>
        /// <remarks>
        /// The condition control indicates whether the input control associated with the validator
        /// control must contain a value.
        /// 
        /// If <seealso cref="ConditionalCheckBoxID"/> is not specified, <seealso cref="ConditionalRequiredFieldValidator"/>
        /// behaves the same as <seealso cref="RequiredFieldValidator"/> (ie, the input field must always contains value).
        /// </remarks>
        public string ConditionalCheckBoxID
        {
            get { return _conditionalCheckBoxID; }
            set { _conditionalCheckBoxID = value; }
        }

        /// <summary>
        /// Indicates whether the input control must contain a value 
        /// when the checkbox specified by <seealso cref="ConditionalCheckBoxID"/>
        /// is checked or is unchecked.
        /// </summary>
        public bool RequiredWhenChecked
        {
            get { return _requiredWhenChecked; }
            set { _requiredWhenChecked = value; }
        }

        
        /// <summary>
        /// Sets or retrieve the specified background color of the input control when the validation fails.
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
            TextBox input = FindControl(ControlToValidate) as TextBox;
            CheckBox ConditionalCheckBox = (String.IsNullOrEmpty(ConditionalCheckBoxID))? null:FindControl(ConditionalCheckBoxID) as CheckBox;

            // Register the javascripts to be used by the validator controls on the client
            string script;

            _evalFunctionName = ClientID + "_Evaluation";

            // Create different javascript based depending on whether the condition checkbox is specified
            if (ConditionalCheckBoxID!= null)
            {
                script =
                    "<script language='javascript'>" + @"
                        function " + _evalFunctionName + @"()
                        {
                                result = true;

                                chkbox = document.getElementById('" + ConditionalCheckBox.ClientID + @"');
                                textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                                extenderCtrl =  document.getElementById('" + this.ClientID + @"');
                                
                                if (chkbox.checked == " + _requiredWhenChecked.ToString().ToLower() + @")
                                {
                                    if (textbox.value==null || textbox.value=='')
                                    {
                                        result = false;
                                    }
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

                                return  result;
                        }

                        </script>
                    ";
            }
            else
            {
                script =
                    "<script language='javascript'>" + @"
                        function " + _evalFunctionName + @"()
                        {

                                result = true;

                                textbox = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');
                                extenderCtrl =  document.getElementById('" + this.ClientID + @"');
                                
                                if (textbox.value==null || textbox.value=='')
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

                                return  result;
                        }

                        </script>
                    ";
            }

            Page.RegisterClientScriptBlock(ClientID, script);

            base.OnInit(e);
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (RenderUplevel)
            {
                // Add the javascript for clien-side validation
                writer.AddAttribute("evaluationfunction", _evalFunctionName);
            }
        }


        /// <summary>
        /// Called during server side validation
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            TextBox input = FindControl(ControlToValidate) as TextBox;

            CheckBox chkbox = String.IsNullOrEmpty(ConditionalCheckBoxID)? null:FindControl(ConditionalCheckBoxID) as CheckBox;

            if (chkbox != null)
            {
                if (chkbox.Checked == _requiredWhenChecked)
                {
                    string value = GetControlValidationValue(this.ControlToValidate);
                    if (String.IsNullOrEmpty(value))
                    {
                        input.BackColor = Color.Yellow;
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                string value = GetControlValidationValue(this.ControlToValidate);
                if (String.IsNullOrEmpty(value))
                {
                    input.BackColor = Color.Yellow;
                    return false;
                }
            }
            return true;
        }

        #endregion Protected Methods
    }

    
}
