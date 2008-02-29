<%@ Control Language="C#" AutoEventWireup="true" Codebehind="StudyDetailsPanel.ascx.cs"
    Inherits="ClearCanvas.ImageServer.Web.Application.StudyDetails.StudyDetailsPanel" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="SeriesGridView.ascx" TagName="SeriesGridView" TagPrefix="uc3" %>
<%@ Register Src="PatientSummaryPanel.ascx" TagName="PatientSummaryPanel" TagPrefix="uc2" %>
<%@ Register Src="StudyDetailsView.ascx" TagName="StudyDetailsView" TagPrefix="clearcanvas" %>
<%@ Register Src="StudyDetailsSecondaryView.ascx" TagName="StudyDetailsSecondaryView"
    TagPrefix="clearcanvas" %>
<%@ Register TagPrefix="clearcanvas" Namespace="ClearCanvas.ImageServer.Web.Common.WebControls.UI"
    Assembly="ClearCanvas.ImageServer.Web.Common" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="cc2" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="conditional">
    <ContentTemplate>
        <asp:Panel runat="server" ID="StudyDetailsPanelContainer">
            <asp:Panel  runat="server" ID="MessagePanel" 
                            style="margin:bottom:10px; border-right: #dcdcdc 1px solid; border-top: #dcdcdc 1px solid; border-left: #dcdcdc 1px solid; border-bottom: #dcdcdc 1px solid; background-color: #ffffcc; text-align:center; padding-right: 10px; padding-left: 10px; padding-bottom: 10px; padding-top: 10px;">
                 <asp:Label ID="ConfirmationMessage" runat="Server" Text="" />
            </asp:Panel>
            
            <uc2:PatientSummaryPanel ID="PatientSummaryPanel" runat="server"></uc2:PatientSummaryPanel>
            <br />             
            <asp:Panel ID="Panel1" runat="server">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <strong>STUDY</strong>
                        </td>
                        <td align="center">
                            
                        </td>
                    </tr>
                </table>
                <asp:Image ID="Image1" runat="server" Visible="false" />
                <hr />
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" CssClass="CSSToolbarPanelContainer">
                <asp:Panel ID="Panel4" runat="server" CssClass="CSSToolbarPanelBorder" Wrap="False">
                    <asp:Panel ID="Panel5" runat="server" CssClass="CSSToolbarContent">
                        <clearcanvas:ToolbarButton ID="DeleteToolbarButton" runat="server" EnabledImageURL="~/images/icons/DeleteEnabled.png"
                            DisabledImageURL="~/images/icons/DeleteDisabled.png" AlternateText="Delete this study"
                            OnClick="DeleteToolbarButton_Click" />
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="StudyDetailsViewPanel">
                <table width="100%" cellpadding="0" cellspacing="0">
                    <tr valign="top">
                        <td width="50%">
                            <clearcanvas:StudyDetailsView ID="StudyDetailsView1" runat="server"></clearcanvas:StudyDetailsView>
                        </td>
                        <td width="50%" valign="top" align="right">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
            <br />
            <asp:Panel ID="Panel2" runat="server">
                <strong>SERIES</strong>
                <asp:Image ID="Image2" runat="server" Visible="false" />
                <hr />
            </asp:Panel>
            <uc3:SeriesGridView ID="SeriesGridView1" runat="server"></uc3:SeriesGridView>
        </asp:Panel>
        
        
           
                                
        <cc2:ConfirmDialog ID="ConfirmDialog1" runat="server" />
        
    </ContentTemplate>
</asp:UpdatePanel>
