<%@ Import namespace="ClearCanvas.ImageServer.Web.Common.Security"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.SessionTimeout" %>

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
        countdownTimer = setTimeout("Countdown()", 2000);
    };
    
    function Countdown()
    {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (updating)
        {
            $("#<%= CountdownEffectPanel.ClientID %>").hide();
		    $("#<%= MessageBanner.ClientID %>").hide();
		    return;
        }
        
        timeLeft = GetSecondsLeft();
        
        if (timeLeft<= 0)
        {
            window.location = redirectPage;
            return;
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
               	    
        if (expiryTime==null) return 0;

        var utcNow = new Date();
        utcNow.setMinutes(utcNow.getMinutes() + utcNow.getTimezoneOffset());
                                             
        timeLeft = Math.round( (expiryTime.getTime() - utcNow.getTime()) / 1000 ) + 1;// give 1 second to ensure when we redirect, the session is really expired

        var displayTimeLeft = new Date();
        displayTimeLeft.setSeconds(displayTimeLeft.getSeconds() + timeLeft);
        window.status  = " [ Session Expiry Time: " + displayTimeLeft.toLocaleString() + " ]";

        return timeLeft;
    }
    
    
    function GetExpiryTime() {
       
        var name = "ImageServer." + loginId + "=";
        var ca = document.cookie.split(';');
        
        for(var i=0;i < ca.length;i++) {
		    var c = ca[i];
		    while (c.charAt(0)==' ') c = c.substring(1,c.length); // trim leading space
		    if (c.indexOf(name) == 0) {
		        return GetDateFromString(c.substring(name.length,c.length));
		    }
	    }   
	    return null;    
    }
    
    function GetDateFromString(value)
    {
        var dateTime = value.split(' ');
        var date = dateTime[0];
        var time = dateTime[1];
                
        date = date.split('-');
        time = time.split(':');
        
        value = new Date();
        
        value.setDate(date[2]);
        value.setMonth(date[1]-1);  //Months start at 0
        value.setFullYear(date[0]);
        value.setHours(time[0]);
        value.setMinutes(time[1]);
        value.setSeconds(time[2]);
                                             
        return value;
    }
    
    function HideSessionWarning()
    {
        hideWarning = true;
        $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
        $("#<%= MessageBanner.ClientID %>").slideUp();
    }
    
    function RefreshWarning()
    {   
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var updating = prm.get_isInAsyncPostBack();
        if (!hideWarning && !updating)
        {
            UpdateCountdownPanel();
            $("#<%= CountdownEffectPanel.ClientID %>:hidden").show();//.animate({height:"30px"});
            $("#<%= MessageBanner.ClientID %>:hidden").show();
            
        }
        else
        {
            $("#<%= CountdownEffectPanel.ClientID %>").slideUp();
		    $("#<%= MessageBanner.ClientID %>").slideUp();
        }
        
        if (!updating)
        {
            if (timeLeft > minCountdownLength)
                countdownTimer = setTimeout("Countdown()", (timeLeft-minCountdownLength)*1000 );
            else if (timeLeft>30)
                countdownTimer = setTimeout("Countdown()", 5*1000 /* every 5 seconds */);
            else
                countdownTimer = setTimeout("Countdown()", 1000 /* every second */);
               
        }
    }
    
    function UpdateCountdownPanel()
    {
        var timeLeft = GetSecondsLeft();
        $("#<%= SessionTimeoutWarningMessage.ClientID %>").html("No activity is detected. For security reasons, this session will end in " + timeLeft + " seconds.");        
    }
    
</script>


        
<asp:UpdatePanel runat="server" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="KeepAliveLink" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <div class="FixedPos">
            <asp:Panel runat="server" ID="MessageBanner" CssClass="MessageBanner">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                    <td>
                    <asp:Panel runat="server" ID="CountdownBanner" CssClass="CountdownBanner">
                        <asp:Label runat="server" ID="SessionTimeoutWarningMessage" CssClass="SessionTimeoutWarningMessage"></asp:Label> 
                        <asp:Button runat="server" ID="KeepAliveLink" Text="Cancel" Font-Size="12px" UseSubmitBehavior="false" OnClientClick="HideSessionWarning()"></asp:Button>           
                    </asp:Panel></td>
                    </tr>
                </table>                
            </asp:Panel>
        </div>
        
        <asp:Panel runat="server" ID="CountdownEffectPanel" Height="40px" CssClass="CountdownEffectPanel"></asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>