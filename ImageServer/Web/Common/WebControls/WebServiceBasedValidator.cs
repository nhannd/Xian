using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Common.WebControls
{
    public class WebServiceBasedValidator : BaseValidator
    {
        private string _servicePath;
        public string ServicePath
        {
            get { return _servicePath; }
            set { _servicePath = value; }
        }

        protected void RenderServiceDiv(System.Web.UI.HtmlTextWriter writer,

                            System.Web.UI.Control Container)
        {

            writer.Write("<span id=\"service\" style=\"behavior:url(webservice.htc);\">");
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (FindControl("service") == null)
            {
                HtmlControl serviceBehaviourDiv = new HtmlGenericControl();
                serviceBehaviourDiv.ID = "service";
                serviceBehaviourDiv.Style.Add("behavior", "url(webservice.htc)");
                
                //serviceBehaviourDiv.SetRenderMethodDelegate(RenderServiceDiv);

                Controls.Add(serviceBehaviourDiv);
            }
        

            string script =
                "<script type='text/javascript'>" + @"

                        function addLoadEvent(func) {
                          var oldonload = window.onload;
                          if (typeof window.onload != 'function') {
                            window.onload = func;
                          } else {
                            window.onload = function() {
                              if (oldonload) {
                                oldonload();
                              }
                              func();
                            }
                          }
                        }



                        function " + EvalFunctionName +
                @"()
                        {              

                            //alert('sending web service request');             
                            var res;
                            callObj = service.createCallOptions();
                            callObj.funcName = 'ValidateFilesystemPath';
                            callObj.async = false;
                            callObj.params = new Array();
                    	    
                            control = document.getElementById('" + GetControlRenderID(ControlToValidate) + @"');


	                        callObj.params.path = control.value;

	                        
                            resObj = service.ValidationServices.callService(parseResult, callObj, res);
                            result = parseResult(resObj);
                            if (!result)
                            {
                                control.style.backgroundColor = '" + InvalidInputBackColor + @"';
                                helpCtrl = document.getElementById('" + GetControlRenderID(PopupHelpControlID) + @"');

                                if (helpCtrl!=null){
                                    helpCtrl.style.visibility='visible';
                                    helpCtrl.alt='" + ErrorMessage + @"';
                                }
                            }
                            else
                            {
                                control.style.backgroundColor = '';
                                
                            }


                            return result;
                        
                        }

    
     
                         function parseResult(res)
                        {
                            if (!res.error) 
	                        {
		                        //alert('Result=' +res.value);
                                return res.value;
	                        }
	                        else
	                        {
		                        //alert(res.errorDetail.string);
                                return false;
	                        }
                        }

        
                </script>";

           

            Page.ClientScript.RegisterClientScriptBlock(GetType(), ClientID, script);

            string baseUrl = Page.Request.Url.Scheme + "://" + Page.Request.Url.Authority +
                             Page.Request.ApplicationPath.TrimEnd('/');

            string initscriptname = ClientID + "_init";

            script = "<script type='text/javascript'>" + @"

                        function initwebservice()
                        {
                            //service.useService('http://localhost:1646/Services/ValidationServices.asmx?WSDL','ValidationServices');
                            service.useService('" + baseUrl + Page.ResolveUrl(ServicePath) + @"?WSDL','ValidationServices');
                            service.onserviceavailable = onserviceavailable();
                       
                       }
                       
                       
                        function onserviceavailable(){
                            //alert('web service ready');
                        }
                        
                       
                        addLoadEvent(function() {
                            //alert('Adding load event');
                            initwebservice();
                        });       
                    </script>";

            Page.ClientScript.RegisterStartupScript(GetType(), "WebServiceInit", script);

            
        }

        protected override bool EvaluateIsValid()
        {
            
            return true;
        }
    }
}
