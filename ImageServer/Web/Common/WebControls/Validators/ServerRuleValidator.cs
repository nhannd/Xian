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
        protected override bool OnServerSideEvaluate()
        {
            String ruleXml = GetControlValidationValue(ControlToValidate);

            if (String.IsNullOrEmpty(ruleXml))
            {
                ErrorMessage = "Server Rule XML must be specified";
                return false;
            }

            XmlDocument theDoc = new XmlDocument();

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

            //string error;
            //if (false == Rule.ValidateRule(ServerRuleTypeEnum.StudyCompress, theDoc, out error))
            //{
                //ErrorMessage = error;
                //return false;
            //}

            return true;
        }
    }
}
