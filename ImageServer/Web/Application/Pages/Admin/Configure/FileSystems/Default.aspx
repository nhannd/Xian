<%@ Page Language="C#" MasterPageFile="~/GlobalMasterPage.master" AutoEventWireup="true"
    EnableEventValidation="false" Codebehind="Default.aspx.cs" Inherits="ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.FileSystems.Default"
    Title="Configure Filesystems" %>

<%@ Register Src="AddEditFileSystemDialog.ascx" TagName="AddEditFileSystemDialog"
    TagPrefix="localAsp" %>
<%@ Register Src="FileSystemsPanel.ascx" TagName="FileSystemsPanel" TagPrefix="localAsp" %>

<asp:Content ID="ContentTitle" ContentPlaceHolderID="MainContentTitlePlaceHolder" runat="server">Configure Filesystems</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server" CssClass="ContentPanel">
                <localAsp:FileSystemsPanel ID="FileSystemsPanel1" runat="server"></localAsp:FileSystemsPanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <localAsp:AddEditFileSystemDialog ID="AddEditFileSystemDialog1" runat="server" />
</asp:Content>

  
