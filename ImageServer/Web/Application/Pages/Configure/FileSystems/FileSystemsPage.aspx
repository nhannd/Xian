<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="FileSystemsPage.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Configure.FileSystems.FileSystemsPage"
    Title="ClearCanvas ImageServer" %>

<%@ Register Src="AddEditFileSystemDialog.ascx" TagName="AddEditFileSystemDialog"
    TagPrefix="uc3" %>
<%@ Register Src="FileSystemsPanel.ascx" TagName="FileSystemsPanel" TagPrefix="uc2" %>

<asp:Content ID="LocationName" ContentPlaceHolderID="LocationNamePlaceHolder" runat="server">Configure > Filesystems</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                <uc2:FileSystemsPanel ID="FileSystemsPanel1" runat="server"></uc2:FileSystemsPanel>
            </asp:Panel>
            <uc3:AddEditFileSystemDialog ID="AddEditFileSystemDialog1" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
