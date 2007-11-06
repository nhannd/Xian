<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="ImageServerWebApplication.Admin.Configuration.DeviceGridView" Codebehind="DeviceGridView.ascx.cs" %>
<%@ Register Src="AddDeviceDialog.ascx" TagName="AddDeviceDialog" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:Panel ID="Panel1" runat="server"  Width="99%">

<asp:Panel ID="Panel2" runat="server" Height="547px" Width="100%" BorderColor="Silver" BorderWidth="1px">
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
                ForeColor="#333333"
                OnRowDataBound="GridView1_RowDataBound" 
                OnDataBound="GridView1_DataBound"
                OnSelectedIndexChanged="GridView1_SelectedIndexChanged"
                OnSelectedIndexChanging="GridView1_SelectedIndexChanging" 
                EmptyDataText="No devices available (Please check the filter settings!)" 
                OnPageIndexChanging="GridView1_PageIndexChanging" 
                PageSize="25" 
                AllowSorting="True" width="100%" CellPadding="2" AllowPaging="True" BackColor="#E0E0E0" CaptionAlign="Top" BorderWidth="1px"
                 >
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <Columns>
                    <asp:BoundField DataField="AETitle" HeaderText="AE Title" >
                    </asp:BoundField>
                    <asp:BoundField DataField="Description" HeaderText="Description" >
                        
                    </asp:BoundField>
                    <asp:BoundField DataField="IPAddress" HeaderText="IP Address" >
                        
                    </asp:BoundField>
                    <asp:BoundField DataField="Port" HeaderText="Port">
                        
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Active">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Active") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="ActiveImage" runat="server" ImageUrl="~/images/unchecked_small.gif"/>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DHCP">
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("DHCP") %>'></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Image ID="DHCPImage" runat="server" ImageUrl="~/images/unchecked_small.gif" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" />
                        
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Partition">
                        <ItemTemplate>
                            <asp:Label ID="ServerParitionLabel" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#EFF3FB" BorderStyle="Solid" BorderWidth="1px" BorderColor="#E0E0E0" />
                <EditRowStyle BackColor="#2461BF" Wrap="True" BorderColor="Yellow" />
                <SelectedRowStyle BackColor="#FF8080" Font-Bold="False" ForeColor="#333333" BorderColor="#0FFFF8" />
                <PagerStyle BackColor="Silver" HorizontalAlign="Center" CssClass="TableHeaderRow" />
                <HeaderStyle CssClass="GridHeader" />
                <AlternatingRowStyle BackColor="White" />
    <PagerTemplate>
        <table width="100%" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <asp:Label ID="PagerDeviceCountLabel" runat="server" Text="Label"></asp:Label></td>
                <td>
                    <asp:Label ID="PagerPagingLabel" runat="server" Text="Label"></asp:Label></td>
                <td align="right">
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:ImageButton ID="PagerPrevImageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                                    ImageUrl="~/images/prev.gif" OnCommand="ImageButton_Command" />                    
                            </td>
                            <td>
                                <asp:ImageButton ID="PagerNextImageButton" runat="server" CommandArgument="Next" CommandName="Page"
                                     ImageUrl="~/images/next.gif" OnCommand="ImageButton_Command" /></td>
                          
                        </tr>
                        
                    </table>
                    
            </tr>
        </table>
    </PagerTemplate>
                
            </asp:GridView>
</asp:Panel>

<asp:Panel ID="PagerPanel" runat="server" BorderColor="Gray" BackColor="Silver" BorderWidth="1px"
    Width="100%" BorderStyle="Solid" >
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center" style="height: 22px" >
                <asp:Label ID="DeviceListingStatusLabel" runat="server" Text="Label"></asp:Label></td>
            <td align="center" style="height: 22px" >
                <asp:Label ID="PageLabel" runat="server" Text="Label"></asp:Label></td>
            <td align="right" style="height: 22px" >
                <asp:ImageButton ID="PrevImageButton" runat="server" CommandArgument="Prev" CommandName="Page"
                    ImageUrl="~/images/prev.gif" OnCommand="ImageButton_Command" />
                <asp:ImageButton ID="NextImageButton" runat="server" CommandArgument="Next" CommandName="Page"
                    ImageUrl="~/images/next.gif" OnCommand="ImageButton_Command" /></td>
        </tr>
    </table>
</asp:Panel>
</asp:Panel>
