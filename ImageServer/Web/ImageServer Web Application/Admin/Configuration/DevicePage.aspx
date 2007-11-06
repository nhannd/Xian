<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPage.master" AutoEventWireup="true" 
    EnableEventValidation="false"
    CodeBehind="DevicePage.aspx.cs" Inherits="ImageServerWebApplication.Admin.Configuration.DevicePage" Title="ClearCanvas ImageServer" %>

<%@ Register Src="EditDeviceDialog.ascx" TagName="EditDeviceDialog" TagPrefix="uc3" %>

<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellpadding="0" cellspacing="0" width="1000px">
        <tr>
            <td class="WindowTitleBar">
            Device Management
            </td>
         </tr>
         <tr>
            <td>
                <asp:Panel runat="server" ID="PageContent"  CssClass="ContentWindow" Height="750px">
                    <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" 
                        CssClass="visoft__tab_xpie7" Width="95%">
                        <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="TabPanel1">
                        <HeaderTemplate>
                        </HeaderTemplate>

                        <ContentTemplate>

                            
                        </ContentTemplate>


                        </cc1:TabPanel>
                    </cc1:TabContainer>
                    <uc2:AddDeviceDialog ID="AddDeviceControl1" runat="server" />
                
                    <uc3:EditDeviceDialog ID="EditDeviceControl1" runat="server" />

                </asp:Panel>
            </td>
         </tr>
         
    </table>
    

</asp:Content>
