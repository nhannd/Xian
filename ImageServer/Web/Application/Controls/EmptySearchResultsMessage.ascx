<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmptySearchResultsMessage.ascx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Controls.EmptySearchResultsMessage" %>

<asp:Panel ID="Panel1" runat="server" CssClass="EmptySearchResultsMessage">
    <asp:Label runat="server" ID="ResultsMessage" Text = "No items found using the provided criteria." />
    <p></p>
    <asp:Panel runat="server"  ID="SuggestionPanel" HorizontalAlign="center">
    <center>
        <table  class="EmptySearchResultsSuggestionPanel">
	        <tr align="left">
	        <td class="EmptySearchResultsSuggestionPanelHeader">
	            <asp:Label runat="server" ID="SuggestionTitle" Text="What can I do?"></asp:Label></td>
	        </tr>
	        <tr align="left" class="EmptySearchResultsSuggestionContent">
	        <td class="EmptySearchResultsSuggestionContent" style="padding:10px;">
	             <asp:PlaceHolder ID="SuggestionPlaceHolder" runat="server"></asp:PlaceHolder>
	        </td>
	        </tr>
	    </table>
        </center>
    </asp:Panel>
    
</asp:Panel>