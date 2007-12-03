using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    abstract public class BaseValidator: Sample.Web.UI.Compatibility.BaseValidator
    {
        // specify the control that will popup the help infomration
        private string _popupHelpControlID="";
        // the background color of the input when the validation fails.
        private string _invalidInputBackColor;

        
        protected string EvalFunctionName
        {
            get { return ClientID + "_Evaluation"; }
        }

        public string PopupHelpControlID
        {
            get { return _popupHelpControlID; }
            set { _popupHelpControlID = value; }
        }

        /// <summary>
        /// Sets or retrieve the specified background color of the input control when the validation fails.
        /// </summary>
        public string InvalidInputBackColor
        {
            get { return _invalidInputBackColor; }
            set { _invalidInputBackColor = value; }
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            base.AddAttributesToRender(writer);

            if (RenderUplevel)
            {
                // Add the javascript for clien-side validation
                writer.AddAttribute("evaluationfunction", EvalFunctionName);
            }
        }

    }
}
