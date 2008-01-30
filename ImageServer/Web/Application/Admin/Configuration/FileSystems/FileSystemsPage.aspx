<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="FileSystemsPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Admin.Configuration.FileSystems.FileSystemsPage"
    Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditFileSystemDialog.ascx" TagName="AddEditFileSystemDialog"
    TagPrefix="uc3" %>
<%@ Register Src="FileSystemsPanel.ascx" TagName="FileSystemsPanel" TagPrefix="uc2" %>
<%@ Register Src="~/Common/ConfirmDialog.ascx" TagName="ConfirmDialog" TagPrefix="uc1" %>
<asp:Content ID="ContentTitle" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    File System Management
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                <uc2:FileSystemsPanel ID="FileSystemsPanel1" runat="server"></uc2:FileSystemsPanel>
            </asp:Panel>
            <uc3:AddEditFileSystemDialog ID="AddEditFileSystemDialog1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
