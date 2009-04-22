<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.SessionTimeout" %>

<!--[if gte IE 5.5]>
<![if lt IE 7]>
<style type="text/css">
.CountdownBanner {
  /* IE5.5+/Win - this is more specific than the IE 5.0 version */
  left: expression( ( ( ignoreMe2 = document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ) ) + 'px' );
  top: expression( ( ( ignoreMe = document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ) ) + 'px' );
  position: absolute;
}

</style>
<![endif]>
<![endif]-->

<script type="text/javascript">
    var countdownTimer;
    var redirectPage = "<%= ResolveClientUrl("~/Pages/Error/TimeoutErrorPage.aspx") %>";
    var loginId = "<%= HttpContext.Current.User.Identity.Name %>";
    var minCountdownLength = <%= MinCountDownDuration.TotalSeconds %>;
    var timeLeft;
    var hideWarning = true;
    Sys.Application.add_load(initCountdownTimer);
    
    function initCountdownTimer(){ 
         hideWarning = true;
         countdownTimer = setTimeout("Countdown()", 1000);
    };
    
    function Countdown()
    {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (updating)
        {
            $("#<%= CountdownEffectPanel.ClientID %>").hide();
		    $("#<%= CountdownBanner.ClientID %>").hide();
		    $("#DarkenBackground").hide();
		    return;
        }
        
        timeLeft = GetSecondsLeft();
        if (timeLeft<= 0)
        {
            window.location = redirectPage;
        }
        else if (timeLeft > minCountdownLength)
        {
            hideWarning = true;
        }
        else
        {
            hideWarning = false;
        }
        
        RefreshWarning();
        
    }
    
    function GetSecondsLeft()
    {
        var expiryTime = GetExpiryTime();
        if (expiryTime==null)
        {
            return 0;
        }
        var now = new Date();
        var localTime = now.getTime();
        var localOffset = now.getTimezoneOffset() * 60000;
        
        var utc = localTime + localOffset;
        
        now = new Date(utc);
        var sessionExpiry = new Date(expiryTime);
        return Math.round( (sessionExpiry.getTime() - now.getTime()) / 1000 ) + 1// give 1 second to ensure when we redirect, the session is really expired;
    }
    
    
    function GetExpiryTime() {
        var name = "ImageServer_" + loginId + "=";
        var ca = document.cookie.split(';');
	    for(var i=0;i < ca.length;i++) {
		    var c = ca[i];
		    while (c.charAt(0)==' ') c = c.substring(1,c.length); // trim leading space
		    
		    if (c.indexOf(name) == 0) {
		        return c.substring(name.length,c.length);
		    }
	    }   
	    return null;    
    }
    
    function HideSessionWarning()
    {
        hideWarning = true;
        $("#<%= CountdownEffectPanel.ClientID %>").hide();
        $("#<%= CountdownBanner.ClientID %>").hide();
		$("#DarkenBackground").hide();
    }
    
    function RefreshWarning()
    {   
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (!hideWarning && !updating)
        {
            UpdateCountdownPanel();
            $("#DarkenBackground").show();
            $("#<%= CountdownBanner.ClientID %>:hidden").show();//slideDown();
            $("#<%= CountdownEffectPanel.ClientID %>:hidden").show();//animate({height:"30px"});
            
        }
        else
        {
            $("#<%= CountdownEffectPanel.ClientID %>").hide();
		    $("#<%= CountdownBanner.ClientID %>").hide();
		    $("#DarkenBackground").hide();
        }
        
        if (!updating)
        {
            countdownTimer = setTimeout("Countdown()", 1000 /* every second */);
        }
    }
    
    function UpdateCountdownPanel()
    {
        var timeLeft = GetSecondsLeft();
        $("#<%= SessionTimeoutWarningMessage.ClientID %>").html("No activity is detected. For security reason, this session will end in " + timeLeft + " seconds.");        
    }
    
</script>


<asp:Panel runat="server" ID="CountdownEffectPanel"></asp:Panel>
        
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="KeepAliveLink" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <div id="DarkenBackground" class="DarkenBackground" style="display:none">
        </div>
        <asp:Panel runat="server" ID="CountdownBanner"  CssClass="CountdownBanner">
            <asp:Label runat="server" ID="SessionTimeoutWarningMessage" CssClass="SessionTimeoutWarningMessage"></asp:Label> 
            <asp:Button runat="server" ID="KeepAliveLink" Text="Refresh" UseSubmitBehavior="false" OnClientClick="HideSessionWarning()"></asp:Button>           
        </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>