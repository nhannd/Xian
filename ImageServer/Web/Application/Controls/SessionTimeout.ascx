<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.SessionTimeout" %>

<script type="text/javascript">

    var waitTime = <%= Int32.Parse(ConfigurationManager.AppSettings.Get("ClientSideTimeout")) %> * 60000;
    var webServicePath = "<%= ResolveClientUrl("~/Services/SessionService.asmx") %>";
    var redirectPage = "<%= ResolveClientUrl("~/Pages/Error/TimeoutErrorPage.aspx") %>";
    var loginId = "<%= HttpContext.Current.User.Identity.Name %>";
    
    var timer = setTimeout("NoActivity()", waitTime);
    
    document.onmousemove = UserActivity;
    if (document.captureEvents) document.captureEvents(Event.MOUSEMOVE);
    
    function UserActivity(e) {
        if(timer != null) {
            clearTimeout ( timer );
            timer = setTimeout("NoActivity()", waitTime);
        }
    }    
    
   function NoActivity() {
                
        var expiryTime = GetExpiryTime();
                       
        if(expiryTime == null) {
            window.location = redirectPage;
        } else {
            var now = new Date();
            var localTime = now.getTime();
            var localOffset = now.getTimezoneOffset() * 60000;
            
            var utc = localTime + localOffset;
            
            now = new Date(utc);
            
            var sessionExpiry = new Date(expiryTime);
                        
            if(now > sessionExpiry) {
                window.location = redirectPage;
            } else {
                clearTimeout ( timer );
                timer = setTimeout("NoActivity()", waitTime);
            }
        }
    }
    
    function GetExpiryTime() {
        var name = "ImageServer_" + loginId + "=";
	    var ca = document.cookie.split(';');
	    for(var i=0;i < ca.length;i++) {
		    var c = ca[i];
		    while (c.charAt(0)==' ') c = c.substring(1,c.length);
		        if (c.indexOf(name) == 0) {
		            return c.substring(name.length,c.length);
		        }
	    }   
	    return null;    
    }
    
</script>