#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Web.Common.WebControls.Validators
{
    /// <summary>
    /// Validator for Server Rules.  Note that this control only works with client side validation.
    /// It will not work properly with just server side validation.
    /// </summary>
    public class ServerRuleValidator : WebServiceValidator
    {
        public string RuleTypeControl
        {
            get
            {
                return ViewState["RULE_TYPE"].ToString();
            } 
            set
            {
                ViewState["RULE_TYPE"] = value;
            }
        }

        protected override bool OnServerSideEvaluate()
        {
            String ruleXml = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(ruleXml))
            {
                ErrorMessage = "Server Rule XML must be specified";
                return false;
            }

            var theDoc = new XmlDocument();

            try
            {
                theDoc.LoadXml(ruleXml);
            }
            catch (Exception e)
            {
                ErrorMessage = "Unable to parse XML: " + e.Message;
                return false;
            }

            //TODO:  When we added "context" validation of rules, ie the rules are validated
            // differently depending on the type of rule, it because impossible for this
            // server side validation to work, because the control doesn't have the rule type
            // when trying to validate input.  The Web service already does the validation before
            // we get to this point, so this should be fine that we can't do the check here.

            string error;
            if (false == Rule.ValidateRule(ServerRuleTypeEnum.GetEnum(RuleTypeControl), theDoc, out error))
            {
                ErrorMessage = error;
                return false;
            }

            return true;
        }
    }
}