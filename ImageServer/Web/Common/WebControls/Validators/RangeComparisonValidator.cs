#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
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
        private string _compareToInputName;
        private string _comparisonControlId;

        #region Public Properties

        /// <summary>
        /// Sets or gets the minimum acceptable value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Sets or gets the maximum acceptable value.
        /// </summary>
        public int MaxValue { get; set; }

        public string ControlToCompare
        {
            get { return _comparisonControlId; }
            set { _comparisonControlId = value; }
        }

        public bool GreaterThan { get; set; }

        public string CompareToInputName
        {
            get
            {
                return String.IsNullOrEmpty(_compareToInputName) ? _comparisonControlId : _compareToInputName;
            }
            set { _compareToInputName = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Called during server-side validation
        /// </summary>
        /// <returns></returns>
        protected override bool OnServerSideEvaluate()
        {
            bool result;
            Decimal value1;
            if (Decimal.TryParse(GetControlValidationValue(ControlToValidate), NumberStyles.Number, null, out value1))
            {
                if (value1 < MinValue || value1 > MaxValue)
                {
                    result = false;
                }
                else
                {
                    Decimal value2;
                    if (Decimal.TryParse(GetControlValidationValue(ControlToCompare), NumberStyles.Number, null,
                                         out value2))
                    {
                        if (GreaterThan)
                        {
                            result = value1 >= value2;
                        }
                        else
                        {
                            result = value1 <= value2;
                        }
                    }
                    else
                    {
                        // can't parse value2... assume value1 is OK
                        result = true;
                    }
                }

                if (result == false && String.IsNullOrEmpty(ErrorMessage))
                {
                    ErrorMessage = String.Format("{0} must be between {1} and {2} and {3} {4}",
                                                 InputName,
                                                 MinValue,
                                                 MaxValue,
                                                 GreaterThan ? "greater than" : "less than",
                                                 CompareToInputName);
                }
            }
            else
            {
                result = false;
                ErrorMessage = String.Format("{0} is not a valid number.", InputName);
            }

            return result;
        }


        protected override void RegisterClientSideValidationExtensionScripts()
        {
            // Register Javascript for client-side validation
            string comparison = GreaterThan ? ">=" : "<=";

            var template =
                new ScriptTemplate(this,
                                   "ClearCanvas.ImageServer.Web.Common.WebControls.Validators.RangeComparisonValidator.js");
            template.Replace("@@COMPARE_INPUT_CLIENTID@@", GetControlRenderID(ControlToCompare));
            template.Replace("@@MIN_VALUE@@", MinValue.ToString());
            template.Replace("@@MAX_VALUE@@", MaxValue.ToString());
            template.Replace("@@COMPARISON_OP@@", comparison);
            template.Replace("@@COMPARE_TO_INPUT_NAME@@", CompareToInputName);
            template.Replace("@@CONDITION_CHECKBOX_CLIENTID@@",
                             ConditionalCheckBox != null ? ConditionalCheckBox.ClientID : "null");
            template.Replace("@@VALIDATE_WHEN_UNCHECKED@@", ValidateWhenUnchecked ? "true" : "false");
            template.Replace("@@IGNORE_EMPTY_VALUE@@", IgnoreEmptyValue ? "true" : "false");


            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID + "_ValidatorClass", template.Script, true);
        }

        #endregion Protected Methods
    }
}