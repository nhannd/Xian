using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.WebControls.Validators;

namespace ClearCanvas.ImageServer.Web.Application.Common
{
    public partial class InvalidInputIndicator : System.Web.UI.UserControl, IInvalidInputIndicator
    {
        private int _referenceCounter = 0;

        public String ImageUrl
        {
            get { return Image.ImageUrl; }
            set { Image.ImageUrl = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ContainerPanel.Attributes.Add("shared", _referenceCounter>1? "true":"false");  
 
            ContainerPanel.Style.Add("display", "block");
            ContainerPanel.Style.Add("visibility", "hidden");   

        }
                
           
        public Control Container
        {
            get { return ContainerPanel; }
        }


        public void Show()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "visible";

        }

        public void Hide()
        {
            ContainerPanel.Style[HtmlTextWriterStyle.Visibility] = "hidden";
             
        }


        public Label TooltipLabel
        {
            get { return HintLabel; }
        }

        

        public void AttachValidator(ClearCanvas.ImageServer.Web.Common.WebControls.Validators.BaseValidator validator)
        {
            _referenceCounter ++;
            validator.InputControl.Attributes.Add("multiplevalidators", _referenceCounter > 1 ? "true" : "false");
        }


        #region IInvalidInputIndicator Members


        public Control TooltipLabelContainer
        {
            get
            {
                return HintPanel;

            }
        }

        #endregion
    }
}